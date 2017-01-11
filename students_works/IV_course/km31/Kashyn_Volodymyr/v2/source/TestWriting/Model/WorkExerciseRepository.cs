using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace TestWriting.Model
{
    public class WorkExerciseRepository : IRepository<WorkExercise>, IDisposable
    {
        OracleConnection connection;

        public WorkExerciseRepository()
        {
            Database db = new Database();
            connection = db.Connect();
        }

        public WorkExerciseRepository(OracleConnection connection)
        {
            this.connection = connection;
        }

        public void Insert(WorkExercise workEx)
        {
            WorkExercise oldWorkEx = this.FindByIndex(workEx.WorkName, workEx.ExerciseName, workEx.Subject, workEx.Theme, workEx.Teacher);
            if (oldWorkEx != null)
            {
                throw new Exception("Record with same Work name, Task name, Subject, Theme and Teacher has been already created");
            }

            OracleCommand command = new OracleCommand
            {
                CommandText = @"INSERT INTO work_exercise
                                    (fk_work_name, fk_exercise_name, fk_subject, fk_theme, fk_teacher, points) 
                                VALUES (:w_name, :e_name, :we_subject, :we_theme, :we_teacher, :we_points)",
                Connection = this.connection
            };

            command.Parameters.Add(":w_name", OracleDbType.Varchar2).Value = workEx.WorkName;
            command.Parameters.Add(":e_name", OracleDbType.Varchar2).Value = workEx.ExerciseName;
            command.Parameters.Add(":we_subject", OracleDbType.Varchar2).Value = workEx.Subject;
            command.Parameters.Add(":we_theme", OracleDbType.Varchar2).Value = workEx.Theme;
            command.Parameters.Add(":we_teacher", OracleDbType.Varchar2).Value = workEx.Teacher;
            command.Parameters.Add(":we_points", OracleDbType.Varchar2).Value = workEx.Points;

            command.ExecuteNonQuery();

        }

        public void Update(WorkExercise workEx)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"UPDATE exercise SET
                                fk_work_name = :w_name, 
                                fk_exercise_name = :e_name, 
                                fk_subject = :we_subject, 
                                fk_theme = :we_theme, 
                                fk_teacher = :we_teacher, 
                                points = :we_points
                               WHERE fk_work_name = :w_name AND fk_exercise_name = :e_name AND fk_subject = :we_subject AND fk_theme = :we_theme AND fk_teacher = :we_teacher",
                Connection = this.connection
            };

            

            command.Parameters.Add(":w_name", OracleDbType.Varchar2).Value = workEx.WorkName;
            command.Parameters.Add(":e_name", OracleDbType.Varchar2).Value = workEx.ExerciseName;
            command.Parameters.Add(":we_subject", OracleDbType.Varchar2).Value = workEx.Subject;
            command.Parameters.Add(":we_theme", OracleDbType.Varchar2).Value = workEx.Theme;
            command.Parameters.Add(":we_teacher", OracleDbType.Varchar2).Value = workEx.Teacher;
            command.Parameters.Add(":we_points", OracleDbType.Varchar2).Value = workEx.Points;
            command.ExecuteNonQuery(); 
        }

        public List<WorkExercise> ListOf(int count = 0)
        {

            OracleCommand command = new OracleCommand("SELECT * FROM work_exercise_list", this.connection);
            if (count > 0)
            {
                command.CommandText += "LIMIT " + count.ToString();
            }

            IDataReader reader = command.ExecuteReader();

            List<WorkExercise> workExs = new List<WorkExercise>();

            while (reader.Read())
            {
                WorkExercise workEx = PopulateEntity(reader);
                workExs.Add(workEx);
            }

            return workExs;
        }

        public WorkExercise FindByIndex(string workName, string taskName, string subject, string theme, string teacherAgreementId)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"SELECT * FROM work_exercise_list 
                                    WHERE w_name    = :param_workname
                                      AND e_name   =  :param_taskname
                                      AND we_subject = :param_subject
                                      AND we_theme = :param_theme
                                      AND we_teacher = :param_teacher",
                Connection = this.connection
            };

            command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = workName;
            command.Parameters.Add(":param_taskname", OracleDbType.Varchar2).Value = taskName;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = subject;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = theme;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = teacherAgreementId;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                WorkExercise workEx = PopulateEntity(reader);
                return workEx;
            }

            return null;

        }

        public List<WorkExercise> FindByWorkAndSubjectAndThemeAndAuthor(string workName, string subject, string theme, string teacherAgreementId, int count = 0)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"SELECT * FROM work_exercise_list 
                                    WHERE w_name    = :param_workname
                                      AND we_subject = :param_subject
                                      AND we_theme = :param_theme
                                      AND we_teacher = :param_teacher",
                Connection = this.connection
            };

            command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = workName;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = subject;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = theme;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = teacherAgreementId;

            IDataReader reader = command.ExecuteReader();

            List<WorkExercise> workExs = new List<WorkExercise>();

            while (reader.Read())
            {
                WorkExercise workEx = PopulateEntity(reader);
                workExs.Add(workEx);
            }

            return workExs;
        }

        public void Delete(WorkExercise workEx)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"DELETE FROM work_exercise  
                                    WHERE w_name    = :param_workname
                                      AND e_name   =  :param_taskname
                                      AND we_subject = :param_subject
                                      AND we_theme = :param_theme
                                      AND we_teacher = :param_teacher",

                Connection = this.connection
            };

            command.Parameters.Add(":param_workname", OracleDbType.Varchar2).Value = workEx.WorkName;
            command.Parameters.Add(":param_taskname", OracleDbType.Varchar2).Value = workEx.ExerciseName;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = workEx.Subject;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = workEx.Theme;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = workEx.Teacher;


            command.ExecuteNonQuery();
        }



        
        //protected WorkExercise PopulateEntity(IDataReader reader)
        //{
        //    WorkExercise workEx = new WorkExercise();

        //    workEx.WorkName = Database.GetStringFromReader(reader, "w_name");
        //    workEx.ExerciseName = Database.GetStringFromReader(reader, "e_name");
        //    workEx.Subject= Database.GetStringFromReader(reader, "we_subject");
        //    workEx.Theme = Database.GetStringFromReader(reader, "we_theme");
        //    workEx.Teacher = Database.GetStringFromReader(reader, "we_teacher");
        //    workEx.Points = Int32.Parse(reader["we_points"].ToString());

        //    return workEx;

        //}

        public static WorkExercise PopulateEntity(IDataReader reader)
        {
            WorkExercise workEx = new WorkExercise();

            workEx.WorkName = Database.GetStringFromReader(reader, "w_name");
            workEx.ExerciseName = Database.GetStringFromReader(reader, "e_name");
            workEx.Subject = Database.GetStringFromReader(reader, "we_subject");
            workEx.Theme = Database.GetStringFromReader(reader, "we_theme");
            workEx.Teacher = Database.GetStringFromReader(reader, "we_teacher");
            workEx.Points = Int32.Parse(reader["we_points"].ToString());

            return workEx;

        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
