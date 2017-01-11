using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace TestWriting.Model
{
    public class StudentRepository : IRepository<Student>, IDisposable
    {
        public OracleConnection Connection { get; set; }
        public OracleTransaction Transaction { get; set; }

        public StudentRepository()
        {
            Database db = new Database();
            Connection = db.Connect();
        }

        public StudentRepository(IsolationLevel isoLevel)
            : this()
        {
            this.Transaction = Connection.BeginTransaction(isoLevel);
        }


        public StudentRepository(OracleConnection connection)
        {
            this.Connection = connection;
        }

        public void Insert(Student student)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"INSERT INTO student 
                                    (student_studentnumber, student_email, student_password, student_firstname, student_lastname, student_groupalias) 
                                VALUES (:s_num, :s_email, :s_password, :s_fname, :s_lname, :s_group)",
                Connection = this.Connection
            };

            command.Parameters.Add(":s_num", OracleDbType.Varchar2).Value = student.StudentNum;
            command.Parameters.Add(":s_email", OracleDbType.Varchar2).Value = student.Email;
            command.Parameters.Add(":s_password", OracleDbType.Varchar2).Value = student.Password;
            command.Parameters.Add(":s_fname", OracleDbType.Varchar2).Value = student.FirstName;
            command.Parameters.Add(":s_lname", OracleDbType.Varchar2).Value = student.LastName;
            command.Parameters.Add(":s_group", OracleDbType.Varchar2).Value = student.GroupAlias;

            command.ExecuteNonQuery();
        }

        public void Update(Student student)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = @"UPDATE student SET
                                student_studentnumber = :s_num, 
                                student_email = :s_email, 
                                student_password = :s_password, 
                                student_firstname = :s_password, 
                                student_lastname = :s_lname, 
                                student_groupalias = :s_group 
                               WHERE student_studentnumber = :s_num OR student_email = :s_email",
                Connection = this.Connection
            };

            command.Parameters.Add(":s_num", OracleDbType.Varchar2).Value = student.StudentNum;
            command.Parameters.Add(":s_email", OracleDbType.Varchar2).Value = student.Email;
            command.Parameters.Add(":s_password", OracleDbType.Varchar2).Value = student.Password;
            command.Parameters.Add(":s_fname", OracleDbType.Varchar2).Value = student.FirstName;
            command.Parameters.Add(":s_lname", OracleDbType.Varchar2).Value = student.LastName;
            command.Parameters.Add(":s_group", OracleDbType.Varchar2).Value = student.GroupAlias;

            command.ExecuteNonQuery(); 
        }

        public List<Student> ListOf(int count = 0)
        {
            OracleCommand command = new OracleCommand("SELECT * FROM student_list", this.Connection);
            if (count > 0)
            {
                command.CommandText += "LIMIT " + count.ToString();
            }

            IDataReader reader = command.ExecuteReader();

            List<Student> students = new List<Student>();

            while (reader.Read())
            {
                Student student = PopulateEntity(reader);
                students.Add(student);
            }

            return students;

        }

        public List<Student> FindByGroup(string groupAlias)
        {

            OracleCommand command = new OracleCommand("SELECT * FROM student_list WHERE st_group = :param_group", this.Connection);


            command.Parameters.Add(":param_group", OracleDbType.Varchar2).Value = groupAlias;

            IDataReader reader = command.ExecuteReader();

            List<Student> students = new List<Student>();

            while (reader.Read())
            {
                Student student = PopulateEntity(reader);
                students.Add(student);
            }

            return students;
        
        
        }

        public Student FindByStudentNumber(string studentNumber)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = "SELECT * FROM student_list WHERE st_num = :param_studentnum",
                Connection = this.Connection
            };

            command.Parameters.Add(":param_studentnum", OracleDbType.Varchar2).Value = studentNumber;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Student student = PopulateEntity(reader);
                return student;
            }

            return null;
        
        }

        public Student FindByEmail(string studentEmail)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = "SELECT * FROM student_list WHERE st_email = :param_studentemail",
                Connection = this.Connection
            };

            command.Parameters.Add(":param_studentemail", OracleDbType.Varchar2).Value = studentEmail;

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Student student = PopulateEntity(reader);
                return student;
            }

            return null;

        }

        public void Delete(Student student)
        {

            OracleCommand command = new OracleCommand
            {
                CommandText = "DELETE FROM student WHERE student_studentnumber = :s_num",
                Connection = this.Connection
            };

            command.Parameters.Add(":s_num", OracleDbType.Varchar2).Value = student.StudentNum;

            command.ExecuteNonQuery();
        }

        protected Student PopulateEntity(IDataReader reader)
        {
            Student student = new Student();

            student.StudentNum = reader.GetString(reader.GetOrdinal("st_num"));
            student.Email = reader.GetString(reader.GetOrdinal("st_email"));
            student.Password = reader.GetString(reader.GetOrdinal("st_pass"));
            student.FirstName = reader.GetString(reader.GetOrdinal("st_fname"));
            student.LastName = reader.GetString(reader.GetOrdinal("st_lname"));
            student.GroupAlias = reader.GetString(reader.GetOrdinal("st_group"));

            return student;
        
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
