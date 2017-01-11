using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace TestWriting.Model
{
    public class WorkAssignmentRepository: IRepository<WorkAssignment>, IDisposable
    {
        OracleConnection connection;
        public OracleTransaction Transaction;

        public WorkAssignmentRepository()
        {
            Database db = new Database();
            connection = db.Connect();
        }

        public WorkAssignmentRepository(IsolationLevel isoLevel)
            : this()
        {
            this.Transaction = connection.BeginTransaction(isoLevel);
        }

        public WorkAssignmentRepository(OracleConnection connection)
        {
            this.connection = connection;
        }

        public void Insert(WorkAssignment workAssignment)
        {

                OracleCommand command = new OracleCommand
                {
                    CommandText = @"INSERT INTO work_assignment (fk_work_name, fk_exercise_name, fk_subject, 
                                            fk_theme, fk_teacher, fk_studentnum, student_answer, work_point)
                                        VALUES (:param_workname, :param_exercisename, :param_subject, :param_theme, 
                                            :param_teacher, :param_studentnum, :param_answer, :param_point)",


                    Connection = this.connection,
                    BindByName = true
                };

                command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = workAssignment.WorkName;
                command.Parameters.Add(":param_exercisename", OracleDbType.Varchar2).Value = workAssignment.ExerciseName;
                command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = workAssignment.Theme;
                command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = workAssignment.Subject;
                command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = workAssignment.Teacher;
                command.Parameters.Add(":param_studentnum", OracleDbType.Varchar2).Value = workAssignment.StudentNum;
                command.Parameters.Add(":param_answer", OracleDbType.Varchar2).Value = workAssignment.StudentAnswer ?? (object)DBNull.Value;
                command.Parameters.Add(":param_point", OracleDbType.Int32).Value = workAssignment.WorkPoint;

                command.ExecuteNonQuery();        
        }

        public void Update(WorkAssignment entity)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"UPDATE work_assignment SET
                                    student_answer = :param_answer ,
                                    work_point = :param_point,
                                    status = :param_status
                               WHERE fk_work_name = :w_name AND fk_exercise_name = :e_name AND fk_subject = :we_subject AND fk_theme = :we_theme AND fk_teacher = :we_teacher AND fk_studentnum = :param_student",
                Connection = this.connection,
                BindByName = true
            };


            command.Parameters.Add(":w_name", OracleDbType.Varchar2).Value = entity.WorkName;
            command.Parameters.Add(":e_name", OracleDbType.Varchar2).Value = entity.ExerciseName;
            command.Parameters.Add(":we_subject", OracleDbType.Varchar2).Value = entity.Subject;
            command.Parameters.Add(":we_theme", OracleDbType.Varchar2).Value = entity.Theme;
            command.Parameters.Add(":we_teacher", OracleDbType.Varchar2).Value = entity.Teacher;
            command.Parameters.Add(":param_student", OracleDbType.Varchar2).Value = entity.StudentNum;
            command.Parameters.Add(":param_point", OracleDbType.Varchar2).Value = entity.WorkPoint;
            command.Parameters.Add(":param_answer", OracleDbType.Varchar2).Value = entity.StudentAnswer;
            command.Parameters.Add(":param_status", OracleDbType.Varchar2).Value = entity.Status;

            command.ExecuteNonQuery();
        }


        public WorkAssignment FindById(string work, string exercise, string subject, string theme, string author, string student)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"SELECT * FROM task_view 
                                                         WHERE 
                                                            theme = :param_theme 
                                                        AND subject =  :param_subject
                                                        AND teacher = :param_teacher 
                                                        AND taskname = :param_taskname
                                                        AND workname = :param_workname 
                                                        AND studentnum = :param_studentnum",
                Connection = this.connection,
                BindByName = true
            };

            command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = work;
            command.Parameters.Add(":param_taskname", OracleDbType.Varchar2).Value = exercise;
            command.Parameters.Add(":param_studentnum", OracleDbType.Varchar2).Value = student;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = theme;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = subject;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = author;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                WorkAssignment workAssignment = PopulateEntityFromTaskView(reader);
                return workAssignment;
            }

            return null;
        }

        public List<WorkAssignment> GetAllTasksAssignedToStudentByWork(string workName, string subject, string theme, string teacher, string studentNum)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"SELECT * FROM task_view 
                                                         WHERE 
                                                            theme = :param_theme 
                                                        AND subject =  :param_subject
                                                        AND teacher = :param_teacher 
                                                        AND workname = :param_workname 
                                                        AND studentnum = :param_studentnum",
                Connection = this.connection,
                BindByName = true
            };

            command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = workName;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = theme;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = subject;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = teacher;
            command.Parameters.Add(":param_studentnum", OracleDbType.Varchar2).Value = studentNum;

            IDataReader reader = command.ExecuteReader();

            List<WorkAssignment> workAssignments = new List<WorkAssignment>();

            while (reader.Read())
            {

                WorkAssignment workAssignment = PopulateEntityFromTaskView(reader);
                workAssignments.Add(workAssignment);
            }

            return workAssignments;

        }

        public List<WorkAssignment> GetByStudent(string studentNum, bool checkedWorks = false)
        {

            OracleCommand command = new OracleCommand("SELECT * FROM work_assignment_grouped WHERE studentnum = :param_studentnum AND work_status = :param_status", this.connection);
            command.BindByName = true;

            command.Parameters.Add(":param_studentnum", OracleDbType.Varchar2).Value = studentNum;
            command.Parameters.Add(":param_status", OracleDbType.Varchar2).Value = checkedWorks ? "Checked" : "To do";

            IDataReader reader = command.ExecuteReader();

            List<WorkAssignment> workAssignments = new List<WorkAssignment>();

            while (reader.Read())
            {

                WorkAssignment workAssignment = PopulateEntityFromWorkAssignmentGrouped(reader);
                workAssignments.Add(workAssignment);
            }

            return workAssignments;
        
        
        }



        public List<WorkAssignment> GetByTeacher(string teacher, bool checkedWorks = false)
        {
            OracleCommand command = new OracleCommand("SELECT * FROM work_assignment_grouped WHERE teacher = :param_teacher AND work_status = :param_status", this.connection);

            command.BindByName = true;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = teacher;
            command.Parameters.Add(":param_status", OracleDbType.Varchar2).Value = checkedWorks ? "Checked" : "To check";

            IDataReader reader = command.ExecuteReader();

            List<WorkAssignment> workAssignments = new List<WorkAssignment>();

            while (reader.Read())
            {

                WorkAssignment workAssignment = PopulateEntityFromWorkAssignmentGrouped(reader);
                workAssignments.Add(workAssignment);
            }

            return workAssignments;


        }

        public static WorkAssignment PopulateEntityFromWorkAssignmentList(IDataReader reader)
        {
            WorkAssignment workAssignment = new WorkAssignment();

            workAssignment.WorkName = Database.GetStringFromReader(reader, "workname");
            workAssignment.ExerciseName = Database.GetStringFromReader(reader, "exercisename");
            workAssignment.Subject = Database.GetStringFromReader(reader, "subject");
            workAssignment.Theme = Database.GetStringFromReader(reader, "theme");
            workAssignment.Teacher = Database.GetStringFromReader(reader, "teacher");
            workAssignment.StudentNum = Database.GetStringFromReader(reader, "studentnum");      
            workAssignment.StudentAnswer = Database.GetStringFromReader(reader, "answer");
            workAssignment.Status = Database.GetStringFromReader(reader, "work_status");
            workAssignment.WorkPoint = Int32.Parse(reader["point"].ToString());

            return workAssignment;

        }


        public static WorkAssignment PopulateEntityFromWorkAssignmentGrouped(IDataReader reader)
        {
            WorkAssignment workAssignment = new WorkAssignment();

            workAssignment.WorkName = Database.GetStringFromReader(reader, "workname");
            workAssignment.ExerciseName = Database.GetStringFromReader(reader, "exercisename");
            workAssignment.Subject = Database.GetStringFromReader(reader, "subject");
            workAssignment.Theme = Database.GetStringFromReader(reader, "theme");
            workAssignment.Teacher = Database.GetStringFromReader(reader, "teacher");
            workAssignment.StudentNum = Database.GetStringFromReader(reader, "studentnum");
            workAssignment.Status = Database.GetStringFromReader(reader, "work_status");
            workAssignment.WorkPoint = Int32.Parse(reader["point"].ToString());

            return workAssignment;

        }

        public static WorkAssignment PopulateEntityFromTaskView(IDataReader reader)
        {
            WorkAssignment workAssignment = new WorkAssignment();

            workAssignment.WorkName = Database.GetStringFromReader(reader, "workname");
            workAssignment.ExerciseName = Database.GetStringFromReader(reader, "taskname");
            workAssignment.ExerciseText = Database.GetStringFromReader(reader, "tasktext");
            workAssignment.ExerciseMaxPoint = Int32.Parse(reader["points"].ToString());
            workAssignment.TeacherAnswer = Database.GetStringFromReader(reader, "taskanswer_teacher");
            workAssignment.StudentAnswer = Database.GetStringFromReader(reader, "taskanswer_student");
            workAssignment.Subject = Database.GetStringFromReader(reader, "subject");
            workAssignment.Theme = Database.GetStringFromReader(reader, "theme");
            workAssignment.Teacher = Database.GetStringFromReader(reader, "teacher");
            workAssignment.StudentNum = Database.GetStringFromReader(reader, "studentnum");
            workAssignment.Status = Database.GetStringFromReader(reader, "status");
            
            workAssignment.WorkPoint = Int32.Parse(reader["setted_point"].ToString());

            return workAssignment;
        }

        public List<WorkAssignment> ListOf(int count = 0)
        {
            throw new NotImplementedException();
        }

        public void Delete(WorkAssignment entity)
        {
            return;
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
