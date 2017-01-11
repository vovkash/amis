using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace TestWriting.Model
{
    public class TeacherRepository : IRepository<Teacher>, IDisposable
    {
        public OracleConnection Connection { get; set; }
        public OracleTransaction Transaction { get; set; }

        public TeacherRepository()
        {
            Database db = new Database();
            Connection = db.Connect();
        }

        public TeacherRepository(IsolationLevel isoLevel)
            : this()
        {
            this.Transaction = Connection.BeginTransaction(isoLevel);
        }

        public TeacherRepository(OracleConnection connection)
        {
            this.Connection = connection;
        }

        public void Insert(Teacher teacher)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"INSERT INTO teacher
                                    (teacher_agreementid, teacher_email, teacher_password, teacher_firstname, teacher_lastname) 
                                VALUES (:t_num, :t_email, :t_password, :t_fname, :t_lname)",
                Connection = this.Connection
            };

            command.Parameters.Add(":t_num", OracleDbType.Varchar2).Value = teacher.AgreementId;
            command.Parameters.Add(":t_email", OracleDbType.Varchar2).Value = teacher.Email;
            command.Parameters.Add(":t_password", OracleDbType.Varchar2).Value = teacher.Password;
            command.Parameters.Add(":t_fname", OracleDbType.Varchar2).Value = teacher.FirstName;
            command.Parameters.Add(":t_lname", OracleDbType.Varchar2).Value = teacher.LastName;

            command.ExecuteNonQuery();
        }

        public void Update(Teacher teacher)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = @"UPDATE teacher SET
                                teacher_agreementid = :t_num, 
                                teacher_email = :t_email, 
                                teacher_password = :t_password, 
                                teacher_firstname = :t_fname, 
                                teacher_lastname = :t_lname 
                               WHERE teacher_agreementid = :t_num OR teacher_email = :t_email",
                Connection = this.Connection
            };

            command.Parameters.Add(":t_num", OracleDbType.Varchar2).Value = teacher.AgreementId;
            command.Parameters.Add(":t_email", OracleDbType.Varchar2).Value = teacher.Email;
            command.Parameters.Add(":t_password", OracleDbType.Varchar2).Value = teacher.Password;
            command.Parameters.Add(":t_fname", OracleDbType.Varchar2).Value = teacher.FirstName;
            command.Parameters.Add(":t_lname", OracleDbType.Varchar2).Value = teacher.LastName;

            command.ExecuteNonQuery(); 
        }

        public List<Teacher> ListOf(int count = 0)
        {
            OracleCommand command = new OracleCommand("SELECT * FROM teacher_list", this.Connection);
            if (count > 0)
            {
                command.CommandText += "LIMIT " + count.ToString();
            }

            IDataReader reader = command.ExecuteReader();

            List<Teacher> teachers = new List<Teacher>();

            while (reader.Read())
            {
                Teacher teacher = PopulateEntity(reader);
                teachers.Add(teacher);
            }

            return teachers;
        }

        public Teacher FindByAgreementId(string agreementId)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = "SELECT * FROM teacher_list WHERE t_agreement = :param_agreementid",
                Connection = this.Connection
            };

            command.Parameters.Add(":param_agreementid", OracleDbType.Varchar2).Value = agreementId;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Teacher teacher = PopulateEntity(reader);
                return teacher;
            }

            return null;

        }

        public Teacher FindByEmail(string teacherEmail)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = "SELECT * FROM teacher_list WHERE t_mail = :param_email",
                Connection = this.Connection
            };

            command.Parameters.Add(":param_email", OracleDbType.Varchar2).Value = teacherEmail;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Teacher teacher = PopulateEntity(reader);
                return teacher;
            }

            return null;

        }

        public void Delete(Teacher teacher)
        {
            OracleCommand command = new OracleCommand
            {
                CommandText = "DELETE FROM teacher WHERE teacher_agreementid = :t_num",
                Connection = this.Connection
            };

            command.Parameters.Add(":t_num", OracleDbType.Varchar2).Value = teacher.AgreementId;

            command.ExecuteNonQuery();
        }

        protected Teacher PopulateEntity(IDataReader reader)
        {
            Teacher teacher = new Teacher();

            teacher.AgreementId = Database.GetStringFromReader(reader, "t_agreement");
            teacher.Email = Database.GetStringFromReader(reader, "t_mail");
            teacher.FirstName = Database.GetStringFromReader(reader, "t_fname");
            teacher.LastName = Database.GetStringFromReader(reader, "t_lname");
            teacher.Password = Database.GetStringFromReader(reader, "t_pass");


            return teacher;

        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
