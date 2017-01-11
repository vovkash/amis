using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace TestWriting.Model
{
    class WorkRepository : IRepository<Work>, IDisposable
    {
        OracleConnection connection;
        public OracleTransaction Transaction = null;

        public WorkRepository()
        {
            Database db = new Database();
            connection = db.Connect();
        }

        public WorkRepository(IsolationLevel isoLevel)
            : this()
        {
            this.Transaction = connection.BeginTransaction(isoLevel);
        }

        public WorkRepository(OracleConnection connection, OracleTransaction Transaction = null)
        {
            this.connection = connection;
            this.Transaction = Transaction;
        }

        public void Insert(Work work)
        {

            Work oldWork = this.FindByIndex(work.Name, work.Subject, work.Theme, work.Teacher);
            if (oldWork != null)
            {
                throw new Exception("Record with same Name, Subject, Theme and Teacher has been already created");
            }

            OracleCommand command = new OracleCommand
            {
                CommandText = @"INSERT INTO work 
                                    (work_name, work_description, work_theme, work_subject, work_teacher) 
                                VALUES (:w_name, :w_description, :w_theme, :w_subject, :w_teacher)",
                Connection = this.connection
            };

            command.Parameters.Add(":w_name", OracleDbType.Varchar2).Value = work.Name;
            command.Parameters.Add(":w_description", OracleDbType.Varchar2).Value = (work.Description ?? (object)DBNull.Value);
            command.Parameters.Add(":w_theme", OracleDbType.Varchar2).Value = work.Theme;
            command.Parameters.Add(":w_subject", OracleDbType.Varchar2).Value = work.Subject;
            command.Parameters.Add(":w_teacher", OracleDbType.Varchar2).Value = work.Teacher;

            command.ExecuteNonQuery();


        }

        public void InsertWithWorkExercises(Work work, List<WorkExercise> exercises)
        {

            this.Insert(work);

            WorkExerciseRepository weRep = new WorkExerciseRepository(this.connection);

            foreach (var we in exercises)
            {
                weRep.Insert(we);
            }
        }

        public void UpdateWithWorkExercises(Work work, List<WorkExercise> exercises)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"DELETE FROM work_exercise  
                                        WHERE fk_work_name    = :param_workname
                                          AND fk_subject = :param_subject
                                          AND fk_theme = :param_theme
                                          AND fk_teacher = :param_teacher",

                Connection = this.connection
            };

            command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = work.Name;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = work.Subject;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = work.Theme;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = work.Teacher;


            command.ExecuteNonQuery();


            this.Update(work);

            WorkExerciseRepository weRep = new WorkExerciseRepository(this.connection);

            foreach (var we in exercises)
            {
                weRep.Insert(we);
            }




        }

        public void AssignToStudent(Work work, string studentNum)
        {
            List<WorkExercise> wes = new List<WorkExercise>();

            WorkExerciseRepository weRep = new WorkExerciseRepository(this.connection);
            wes = weRep.FindByWorkAndSubjectAndThemeAndAuthor(work.Name, work.Subject, work.Theme, work.Teacher);
            

            if (wes.Count < 1)
            {
                throw new Exception("No task assign!");
            }

            WorkAssignmentRepository waRep = new WorkAssignmentRepository(this.connection);

            foreach (var we in wes)
            {
                WorkAssignment wa = new WorkAssignment(we);
                wa.StudentNum = studentNum;
                waRep.Insert(wa);
            }
        }



        public void AssignToGroup(Work work, string group)
        {
            string error = "";

            StudentRepository stRep = new StudentRepository(this.connection);

            List<Student> students = stRep.FindByGroup(group);
            List<Student> notAssigned = new List<Student>();


            foreach (var student in students)
            {
                try
                {
                    AssignToStudent(work, student.StudentNum);

                }
                catch (Exception)
                {
                    error += student.ToString() + "\n";
                }
            }




            if (error != "")
            {
                throw new Exception("Next students were not assigned to this work because they have already assigned to it:\n" + error);
            }



        }

        public void Update(Work work)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"UPDATE work SET
                                work_name = :w_name, 
                                work_description = :w_description, 
                                work_theme = :w_theme, 
                                work_subject = :w_subject,
                                work_teacher = :w_teacher
                               WHERE work_name = :w_name AND work_theme = :w_theme AND work_subject = :w_subject AND work_teacher = :w_teacher ",
                Connection = this.connection
            };

            command.Parameters.Add(":w_name", OracleDbType.Varchar2).Value = work.Name;
            command.Parameters.Add(":w_description", OracleDbType.Varchar2).Value = (work.Description ?? (object)DBNull.Value);
            command.Parameters.Add(":w_theme", OracleDbType.Varchar2).Value = work.Theme;
            command.Parameters.Add(":w_subject", OracleDbType.Varchar2).Value = work.Subject;
            command.Parameters.Add(":w_teacher", OracleDbType.Varchar2).Value = work.Teacher;

            command.ExecuteNonQuery();
        }

        public List<Work> ListOf(int count = 0)
        {


            OracleCommand command = new OracleCommand("SELECT * FROM work_list", this.connection);
            if (count > 0)
            {
                command.CommandText += "LIMIT " + count.ToString();
            }

            IDataReader reader = command.ExecuteReader();

            List<Work> works = new List<Work>();

            while (reader.Read())
            {
                Work work = PopulateEntity(reader);
                works.Add(work);
            }

            return works;
        }

        public Work FindByIndex(string workName, string subject, string theme, string teacherAgreementId)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"SELECT * FROM work_list 
                                    WHERE w_name    = :param_workname
                                      AND w_theme   = :param_theme
                                      AND w_subject = :param_subject
                                      AND w_teacher = :param_teacher",
                Connection = this.connection
            };

            command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = workName;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = theme;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = subject;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = teacherAgreementId;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Work work = PopulateEntity(reader);
                return work;
            }

            return null;

        }

        public void Delete(Work work)
        {
            if (Transaction == null)
            {
                Transaction = connection.BeginTransaction(IsolationLevel.Serializable);
            }

            OracleCommand command = new OracleCommand
            {
                CommandText = @"DELETE FROM work  
                                    WHERE work_name   =  :param_taskname
                                      AND work_theme   = :param_theme
                                      AND work_subject = :param_subject
                                      AND work_teacher = :param_teacher",

                Connection = this.connection
            };

            command.Parameters.Add(":param_taskname", OracleDbType.Varchar2).Value = work.Name;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = work.Theme;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = work.Subject;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = work.Teacher;

            command.ExecuteNonQuery();

            Transaction.Commit();
        }

        protected Work PopulateEntity(IDataReader reader)
        {
            Work work = new Work();

            work.Name = Database.GetStringFromReader(reader, "w_name");
            work.Description = Database.GetStringFromReader(reader, "w_description");
            work.Theme = Database.GetStringFromReader(reader, "w_theme");
            work.Subject = Database.GetStringFromReader(reader, "w_subject");
            work.Teacher = Database.GetStringFromReader(reader, "w_teacher");


            return work;

        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
