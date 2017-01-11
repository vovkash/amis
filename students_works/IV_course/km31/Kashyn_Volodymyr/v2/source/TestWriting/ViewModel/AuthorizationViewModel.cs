using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TestWriting.Model;
using System.Data;

namespace TestWriting.ViewModel
{
    public class AuthorizationViewModel : PropertyChangedMainViewModel, IDataErrorInfo
    {
        public enum AuthType
        {
            Login,
            Register,
            ProfileEdit
        };


        AuthType accessType = AuthType.Register;

        string oldpassword;

        User user;
        Session.Role userRole;

        string password = "";
        string passwordreenter = "";


        public User User
        {
            get { return user; }
            set
            {
                user = value;
                oldpassword = user.Password;
                NotifyPropertyChanged("User");
                NotifyPropertyChanged("Email");
                NotifyPropertyChanged("UniqueNumber");
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("Lastname");
                NotifyPropertyChanged("Group");
            }

        }

        public Session.Role UserRole
        {
            get { return userRole; }
            set { userRole = value; }
        }

        public string Email
        {
            get { return User.Email; }
            set
            {
                User.Email = value;
                NotifyPropertyChanged("Email");
            }
        }

        public string UniqueNumber
        {
            get { return User.UniqueNumber; }
            set
            {
                User.UniqueNumber = value;
                NotifyPropertyChanged("UniqueNumber");
            }
        }

        public SecureString Password
        {
            set
            {
                password = this.SecureStringToString(value);
                if (password.Length < 1)
                {
                    User.Password = "";
                }
                else
                {
                    User.Password = Database.CalculateMD5Hash(password);
                }
                NotifyPropertyChanged("Password");
            }
        }

        public SecureString Passwordreenter
        {
            set
            {
                passwordreenter = this.SecureStringToString(value);
                NotifyPropertyChanged("Passwordreenter");
            }
        }


        public string Name
        {
            get { return User.FirstName; }
            set
            {
                User.FirstName = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Lastname
        {
            get { return User.LastName; }
            set
            {
                User.LastName = value;
                NotifyPropertyChanged("Lastname");
            }
        }

        public string Group
        {
            get
            {
                if (User is Student)
                {
                    return (User as Student).GroupAlias;
                }
                return "";
            }
            set
            {
                if (User is Student)
                {
                    (User as Student).GroupAlias = value;
                    NotifyPropertyChanged("Group");
                }


            }
        }


        public AuthType AccessType
        {
            get { return accessType; }
            set
            {
                accessType = value;
                NotifyPropertyChanged("EmailEnabled");
                NotifyPropertyChanged("ReenterPasswordVisible");
                NotifyPropertyChanged("NamesVisible");
                NotifyPropertyChanged("GroupVisible");
                NotifyPropertyChanged("GroupEnabled");
                NotifyPropertyChanged("RegisterMeVisible");
                NotifyPropertyChanged("UniqueNumberVisible");

            }
        }

        public bool EmailEnabled
        {
            get
            {
                if (accessType == AuthType.ProfileEdit)
                    return false;
                else
                    return true;
            }
        }

        public bool ReenterPasswordVisible
        {
            get
            {
                if (accessType == AuthType.Login)
                    return false;
                else
                    return true;
            }
        }

        public bool NamesVisible
        {
            get
            {
                if (accessType == AuthType.Login)
                    return false;
                else
                    return true;
            }
        }


        public bool UniqueNumberVisible
        {
            get
            {
                if (accessType == AuthType.Login)
                    return false;
                else
                    return true;
            }

        }

        public bool GroupVisible
        {
            get
            {
                if (accessType == AuthType.Login || !(User is Student))
                    return false;
                else
                    return true;
            }

        }

        public bool GroupEnabled
        {
            get
            {
                if (accessType == AuthType.Register)
                    return true;
                else
                    return false;
            }

        }

        public bool RegisterMeVisible
        {
            get
            {
                if (accessType == AuthType.Login)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string Error
        {
            get
            {
                string error = "";
                foreach (string property in new string[] { "Email", "UniqueNumber", "Name", "Lastname", "Password", "Passwordreenter", "Group" })
                {
                    string s = this[property];
                    if (s != null) // there is an error
                        error += s + "\n";
                }

                return error;
            }
        }

        public bool IsValid
        {
            get
            {
                foreach (string property in new string[] { "Email", "UniqueNumber", "Name", "Lastname", "Password", "Passwordreenter", "Group" })
                {

                    if (this[property] != null) // there is an error
                        return false;
                }

                return true;
            }
        }


        public string this[string columnName]
        {
            get
            {

                if (columnName == "Email")
                {
                    bool result = Regex.IsMatch(Email.ToString(), @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                            && Regex.IsMatch(Email.ToString(), @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");

                    if (result)
                    {
                        return null;
                    }
                    else
                    {
                        return "Entered email is invalid! Please try again";
                    }

                }

                if (columnName == "UniqueNumber")
                {
                    if ((AccessType == AuthType.Register || AccessType == AuthType.ProfileEdit) && UniqueNumber.Length < 1)
                    {
                        if (User is Student)
                        {
                            return "Student number field is mandatory!";
                        }

                        if (User is Teacher)
                        {
                            return "Agreement field is mandatory!";
                        }
                    }

                }

                if (columnName == "Password")
                {
                    if (AccessType == AuthType.Register && password.Length < 6)
                    {
                        return "Password should have at least 6 characters!";
                    }

                    if (AccessType == AuthType.ProfileEdit && password.Length > 0 && password.Length < 6)
                    {
                        return "Password should have at least 6 characters!";
                    }


                }

                if (columnName == "Passwordreenter")
                {
                    if ((AccessType == AuthType.Register || AccessType == AuthType.ProfileEdit) && password.Length > 0 && !String.Equals(password, passwordreenter))
                    {
                        return "Entered passwords should be the same!";
                    }

                }

                if (columnName == "Name" && this.NamesVisible)
                {
                    if (Name.Length < 1)
                    {
                        return "The name field is mandatory!";
                    }

                    Regex regex = new Regex(@"[A-ZА-Яa-zа-яєії '-]+$");
                    if (!regex.IsMatch(Name) || Name.Contains("--") || Name.Contains("''") || Name.First() == '\'' || Name.First() == '-' || Name.Last() == '\'' || Name.Last() == '-')
                    {
                        return "Entered data is invalid. It should contains only letters and symbols ' -";
                    }

                }

                if (columnName == "Lastname" && this.NamesVisible)
                {
                    if (Lastname.Length < 1)
                    {
                        return "The last name field is mandatory!";
                    }

                    Regex regex = new Regex(@"[A-ZА-Яa-zа-яєії '-]+$");
                    if (!regex.IsMatch(Lastname) || Lastname.Contains("--") || Lastname.Contains("''") || Lastname.First() == '\'' || Lastname.First() == '-' || Lastname.Last() == '\'' || Lastname.Last() == '-')
                    {
                        return "Entered data is invalid. It should contains only letters and symbols ' -";
                    }

                }

                if (GroupVisible && columnName == "Group")
                {
                    if (Group == null || Group.Length < 1)
                    {
                        return "Group field is mandatory!";
                    }
                }


                return null;
            }

        }


        public AuthorizationViewModel()
        {


            if (Session.HasRole(Session.Role.Teacher))
            {
                User = new Teacher();
            }
            else
            {
                User = new Student();
            }

            this.Email = "";
            this.Name = "";
            this.Lastname = "";
            this.UniqueNumber = "";
        }

        public AuthorizationViewModel(User user)
        {
            User = user;

            

            this.Email = "";
            this.Name = "";
            this.Lastname = "";
            this.UniqueNumber = "";
        }

        public void Login()
        {

            Session.Roles.Clear();

            using (StudentRepository studentRepository = new StudentRepository(IsolationLevel.ReadCommitted))
            {
                TeacherRepository teacherRepository = new TeacherRepository(studentRepository.Connection);

                User checkUser = studentRepository.FindByEmail(User.Email);
                Teacher checkTeacher = teacherRepository.FindByEmail(User.Email);
                if ((checkUser == null || !String.Equals(checkUser.Password, User.Password, StringComparison.OrdinalIgnoreCase))
                 && (checkTeacher == null || !String.Equals(checkTeacher.Password, User.Password, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception("User wasn't found with entered credentials.");
                }

                if (checkTeacher != null)
                {
                    Session.Roles.Add(Session.Role.Teacher);
                    Session.User = checkTeacher;
                }
                else
                    if (checkUser != null)
                    {
                        Session.Roles.Add(Session.Role.Student);
                        Session.User = checkUser;
                    }

                studentRepository.Transaction.Commit();

            }

            if (Session.Roles.Count < 1)
            {
                throw new Exception("Cant autorize user. Please try again or send request to system administrator.");
            }

            Session.LoginTime = DateTime.Now;
        }

        public bool Register()
        {
            User checkUser;


            using (StudentRepository studentRepository = new StudentRepository(IsolationLevel.Serializable))
            {
                try
                {

                    checkUser = studentRepository.FindByEmail(User.Email);
                    if (checkUser != null)
                    {
                        throw new Exception("User with the same e-mail has been already registered!");

                    }

                    TeacherRepository tRep = new TeacherRepository(studentRepository.Connection);

                    checkUser = tRep.FindByEmail(User.Email);

                    if (checkUser != null)
                    {
                        throw new Exception("User with the same e-mail has been already registered!");

                    }

                    checkUser = studentRepository.FindByStudentNumber(User.UniqueNumber);

                    if (checkUser != null)
                    {
                        throw new Exception("User with the same Student number has been already registered!");
                    }


                    studentRepository.Insert(User as Student);


                    studentRepository.Transaction.Commit();
                }
                catch (Exception exc)
                {
                    studentRepository.Transaction.Rollback();
                    throw new Exception(exc.Message);
                }
            }


            return true;
        }

        public bool UpdateProfile()
        {

            if (User.Password == null || User.Password.Length < 1)
            {
                User.Password = oldpassword;
            }

            if (User is Student)
            {

                using (StudentRepository StudentRepository = new StudentRepository(IsolationLevel.Serializable))
                {
                    StudentRepository.Update(User as Student);
                    StudentRepository.Transaction.Commit();
                }
            }

            if (User is Teacher)
            {
                using (TeacherRepository TeacherRepository = new TeacherRepository(IsolationLevel.Serializable))
                {
                    TeacherRepository.Update(User as Teacher);
                    TeacherRepository.Transaction.Commit();
                }

            }

            return true;
        }

        String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

    }
}
