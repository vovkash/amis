using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace TestWriting.Model
{
    public class ExerciseRepository : IRepository<Exercise>, IDisposable
    {
        public OracleConnection Connection { get; set; }
        public OracleTransaction Transaction { get; set; }

        public ExerciseRepository()
        {
            Database db = new Database();
            Connection = db.Connect();
        }


        public void Insert(Exercise exercise)
        {

            Exercise oldEx = this.FindByIndex(exercise.Name, exercise.Subject, exercise.Theme, exercise.CreatedBy);
            if (oldEx != null)
            {
                throw new Exception("Record with same Name, Subject, Theme and Teacher has been already created");
            }

            OracleCommand command = new OracleCommand
            {
                CommandText = @"INSERT INTO exercise 
                                    (exercise_name, exercise_description, exercise_theme, exercise_subject, exercise_task, exercise_answer, exercise_teacher) 
                                VALUES (:e_name, :e_description, :e_theme, :e_subject, :e_task, :e_answer, :e_teacher)",
                Connection = this.Connection
            };

            command.Parameters.Add(":e_name", OracleDbType.Varchar2).Value = exercise.Name;
            command.Parameters.Add(":e_description", OracleDbType.Varchar2).Value = (exercise.Description ?? (object)DBNull.Value);
            command.Parameters.Add(":e_theme", OracleDbType.Varchar2).Value = exercise.Theme;
            command.Parameters.Add(":e_subject", OracleDbType.Varchar2).Value = exercise.Subject;
            command.Parameters.Add(":e_task", OracleDbType.Varchar2).Value = exercise.Task;
            command.Parameters.Add(":e_answer", OracleDbType.Varchar2).Value = (exercise.Answer ?? (object)DBNull.Value);
            command.Parameters.Add(":e_answer", OracleDbType.Varchar2).Value = exercise.CreatedBy;

            command.ExecuteNonQuery();
        }

        public void Update(Exercise exercise)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"UPDATE exercise SET
                                exercise_name = :e_name, 
                                exercise_description = :e_description, 
                                exercise_theme = :e_theme, 
                                exercise_subject = :e_subject, 
                                exercise_task = :e_task, 
                                exercise_answer = :e_answer,
                                exercise_teacher = :e_teacher
                               WHERE exercise_name = :e_name AND exercise_theme = :e_theme AND exercise_subject = :e_subject AND exercise_teacher = :e_teacher",
                Connection = this.Connection
            };

            command.Parameters.Add(":e_name", OracleDbType.Varchar2).Value = exercise.Name;
            command.Parameters.Add(":e_description", OracleDbType.Varchar2).Value = exercise.Description;
            command.Parameters.Add(":e_theme", OracleDbType.Varchar2).Value = exercise.Theme;
            command.Parameters.Add(":e_subject", OracleDbType.Varchar2).Value = exercise.Subject;
            command.Parameters.Add(":e_task", OracleDbType.Varchar2).Value = exercise.Task;
            command.Parameters.Add(":e_answer", OracleDbType.Varchar2).Value = exercise.Answer;
            command.Parameters.Add(":e_teacher", OracleDbType.Varchar2).Value = exercise.CreatedBy;

            command.ExecuteNonQuery(); 
        }

        public List<Exercise> ListOf(int count = 0)
        {
            OracleCommand command = new OracleCommand("SELECT * FROM exercise_list", this.Connection);
            if (count > 0)
            {
                command.CommandText += "LIMIT " + count.ToString();
            }

            IDataReader reader = command.ExecuteReader();

            List<Exercise> exercises = new List<Exercise>();

            while (reader.Read())
            {
                Exercise exercise = PopulateEntity(reader);
                exercises.Add(exercise);
            }

            return exercises;
        }

        public Exercise FindByIndex(string taskName, string subject, string theme, string teacherAgreementId)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"SELECT * FROM exercise_list 
                                    WHERE e_name    = :param_taskname
                                      AND e_theme   = :param_theme
                                      AND e_subject = :param_subject
                                      AND e_teacher = :param_teacher",
                Connection = this.Connection
            };

            command.Parameters.Add(":param_taskname", OracleDbType.Varchar2).Value = taskName;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = theme;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = subject;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = teacherAgreementId;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Exercise exercise = PopulateEntity(reader);
                return exercise;
            }

            return null;

        }

        public List<Exercise> FindBySubjectAndThemeAndAuthor(string subject, string theme, string author, int count = 0)
        {
            OracleCommand command = new OracleCommand(@"SELECT * FROM exercise_list 
                                    WHERE e_theme   = :param_theme
                                      AND e_subject = :param_subject
                                      AND e_teacher  = :param_teacher", 
                                                        this.Connection);
            if (count > 0)
            {
                command.CommandText += "LIMIT " + count.ToString();
            }

            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = theme;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = subject;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = author;


            IDataReader reader = command.ExecuteReader();

            List<Exercise> exercises = new List<Exercise>();

            while (reader.Read())
            {
                Exercise exercise = PopulateEntity(reader);
                exercises.Add(exercise);
            }

            return exercises;        
        }


       

        public void Delete(Exercise exercise)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"DELETE FROM exercise  
                                    WHERE exercise_name   =  :param_taskname
                                      AND exercise_theme   = :param_theme
                                      AND exercise_subject = :param_subject
                                      AND exercise_teacher = :param_teacher",

                Connection = this.Connection
            };

            command.Parameters.Add(":param_taskname", OracleDbType.Varchar2).Value = exercise.Name;
            command.Parameters.Add(":param_theme", OracleDbType.Varchar2).Value = exercise.Theme;
            command.Parameters.Add(":param_subject", OracleDbType.Varchar2).Value = exercise.Subject;
            command.Parameters.Add(":param_teacher", OracleDbType.Varchar2).Value = exercise.CreatedBy;


            command.ExecuteNonQuery();
        }

        protected Exercise PopulateEntity(IDataReader reader)
        {
            Exercise exercise = new Exercise();

            exercise.Name        = Database.GetStringFromReader(reader, "e_name");
            exercise.Description = Database.GetStringFromReader(reader, "e_description");
            exercise.Theme       = Database.GetStringFromReader(reader, "e_theme");
            exercise.Subject     = Database.GetStringFromReader(reader, "e_subject");
            exercise.Task        = Database.GetStringFromReader(reader, "e_task");
            exercise.Answer      = Database.GetStringFromReader(reader, "e_answer");
            exercise.CreatedBy   = Database.GetStringFromReader(reader, "e_teacher");

            return exercise;

        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
