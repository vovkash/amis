
--
-- STUDENT
--
  CREATE TABLE "STUDENT" 
   (	"STUDENT_EMAIL" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"STUDENT_FIRSTNAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"STUDENT_LASTNAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"STUDENT_PASSWORD" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"STUDENT_GROUPALIAS" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"STUDENT_STUDENTNUMBER" VARCHAR2(50 CHAR),
	CONSTRAINT "STUDENT_STUDENTNUM_PK" PRIMARY KEY ("STUDENT_STUDENTNUMBER") ENABLE,
	CONSTRAINT "STUDENT_EMAIL_UNIQUE" UNIQUE ("STUDENT_EMAIL") ENABLE
   );
--
-- TEACHER
--
  CREATE TABLE "TEACHER" 
   (	"TEACHER_AGREEMENTID" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"TEACHER_EMAIL" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"TEACHER_FIRSTNAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"TEACHER_LASTNAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"TEACHER_PASSWORD" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	CONSTRAINT "TEACHER_AGREEMENTID_PK" PRIMARY KEY ("TEACHER_AGREEMENTID") ENABLE,
	CONSTRAINT "TEACHER_EMAIL_UNIQUE" UNIQUE ("TEACHER_EMAIL") ENABLE
   );
--
-- EXERCISE
--
  CREATE TABLE "EXERCISE" 
   (	"EXERCISE_NAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"EXERCISE_DESCRIPTION" VARCHAR2(50 CHAR),
	"EXERCISE_SUBJECT" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"EXERCISE_THEME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"EXERCISE_TASK" VARCHAR2(300 CHAR) NOT NULL ENABLE,
	"EXERCISE_ANSWER" VARCHAR2(300 CHAR),
	"EXERCISE_TEACHER" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	CONSTRAINT "EXERCISE_IDX_PK" PRIMARY KEY ("EXERCISE_NAME","EXERCISE_SUBJECT","EXERCISE_THEME","EXERCISE_TEACHER") ENABLE
   );
--
-- WORK
--
  CREATE TABLE "WORK" 
   (	"WORK_NAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"WORK_DESCRIPTION" VARCHAR2(50 CHAR),
	"WORK_SUBJECT" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"WORK_THEME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"WORK_TEACHER" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	CONSTRAINT "CONSTRAINT1" PRIMARY KEY ("WORK_NAME","WORK_SUBJECT","WORK_THEME","WORK_TEACHER") ENABLE
   );
--
-- WORK_ASSIGNMENT
--
  CREATE TABLE "WORK_ASSIGNMENT" 
   (	"FK_WORK_NAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_EXERCISE_NAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_SUBJECT" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_THEME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_TEACHER" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_STUDENTNUM" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"STUDENT_ANSWER" VARCHAR2(500 CHAR),
	"WORK_POINT" NUMBER,
	"STATUS" VARCHAR2(50 CHAR) DEFAULT 'To do',
	CONSTRAINT "WORK_ASSIGNMENT_PK" PRIMARY KEY ("FK_EXERCISE_NAME","FK_STUDENTNUM","FK_SUBJECT","FK_TEACHER","FK_THEME","FK_WORK_NAME") ENABLE
   );
--
-- WORK_EXERCISE
--
  CREATE TABLE "WORK_EXERCISE" 
   (	"FK_WORK_NAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_EXERCISE_NAME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_SUBJECT" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_THEME" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"FK_TEACHER" VARCHAR2(50 CHAR) NOT NULL ENABLE,
	"POINTS" NUMBER NOT NULL ENABLE,
	CONSTRAINT "WORK_EXERCISE_PK" PRIMARY KEY ("FK_EXERCISE_NAME","FK_SUBJECT","FK_TEACHER","FK_THEME","FK_WORK_NAME") ENABLE
   );
--
-- EXERCISE_LIST
--
CREATE OR REPLACE FORCE VIEW "EXERCISE_LIST" 
  AS 
  SELECT exercise_name AS e_name, exercise_description AS e_description, exercise_subject AS e_subject, exercise_theme AS e_theme, exercise_task AS e_task, exercise_answer AS e_answer, exercise_teacher AS e_teacher FROM exercise
  WITH READ ONLY;
--
-- STUDENT_LIST
--
CREATE OR REPLACE FORCE VIEW "STUDENT_LIST" ("ST_NUM", "ST_EMAIL", "ST_FNAME", "ST_LNAME", "ST_PASS", "ST_GROUP") AS 
  SELECT student_studentnumber, student_email, student_firstname, student_lastname, student_password, student_groupalias
		FROM student;
--
-- TEACHER_LIST
--
CREATE OR REPLACE FORCE VIEW "TEACHER_LIST" 
  AS 
  SELECT teacher_agreementid as t_agreement, teacher_email as t_mail, teacher_firstname as t_fname, teacher_lastname as t_lname, teacher_password as t_pass FROM teacher
  WITH READ ONLY;
--
-- WORK_ASSIGNMENT_GROUPED
--
CREATE OR REPLACE FORCE VIEW "WORK_ASSIGNMENT_GROUPED" 
  AS 
  SELECT fk_work_name AS workname, count(fk_exercise_name) AS exercisename, fk_subject AS subject, fk_theme AS theme, fk_teacher AS teacher, fk_studentnum AS studentnum, sum(work_point) AS point, status AS work_status  

FROM WORK_ASSIGNMENT 

GROUP BY  fk_studentnum, fk_work_name, fk_subject, fk_theme, fk_teacher,  status
  WITH READ ONLY;
--
-- WORK_ASSIGNMENT_LIST
--
CREATE OR REPLACE FORCE VIEW "WORK_ASSIGNMENT_LIST" 
  AS 
  SELECT fk_work_name AS workname, fk_exercise_name AS exercisename, fk_subject AS subject, fk_theme AS theme, fk_teacher AS teacher, fk_studentnum AS studentnum, work_point AS point, student_answer as answer, status AS work_status  FROM WORK_ASSIGNMENT
  WITH READ ONLY;
--
-- WORK_EXERCISE_LIST
--
CREATE OR REPLACE FORCE VIEW "WORK_EXERCISE_LIST" 
  AS 
  SELECT fk_work_name AS w_name, fk_exercise_name AS e_name, fk_subject as we_subject, fk_theme as we_theme, fk_teacher AS we_teacher, points AS we_points FROM work_exercise
  WITH READ ONLY;
--
-- TASK_VIEW
--
CREATE OR REPLACE FORCE VIEW "TASK_VIEW" 
  AS 
  SELECT 
exercise_list.e_name AS taskname, exercise_list.e_task AS tasktext, exercise_list.e_answer AS taskanswer_teacher, work_exercise_list.we_Points as Points, work_exercise_list.w_name AS workname, work_assignment_list.studentnum AS studentnum,
work_assignment_list.subject AS subject, work_assignment_list.theme AS theme, exercise_list.e_teacher AS teacher,
work_assignment_list.answer AS taskanswer_student, work_assignment_list.work_status AS status, work_assignment_list.point AS setted_point

FROM exercise_list 
                                                     INNER JOIN 
                                                      work_exercise_list ON 
                                                            exercise_list.e_name = work_exercise_list.e_name 
                                                        AND exercise_list.e_subject =work_exercise_list.we_subject
                                                        AND exercise_list.e_theme = work_exercise_list.we_theme
                                                        AND exercise_list.e_teacher = work_exercise_list.we_teacher
                                                     INNER JOIN 
                                                       work_assignment_list ON
                                                            work_assignment_list.exercisename = work_exercise_list.e_name 
                                                        AND work_assignment_list.subject =work_exercise_list.we_subject
                                                        AND work_assignment_list.theme = work_exercise_list.we_theme
                                                        AND work_assignment_list.teacher = work_exercise_list.we_teacher
                                                        AND work_assignment_list.workname = work_exercise_list.w_name
  WITH READ ONLY;
--
-- WORK_LIST
--
CREATE OR REPLACE FORCE VIEW "WORK_LIST" 
  AS 
  SELECT work_name AS w_name, work_description AS w_description, work_subject AS w_subject, work_theme AS w_theme , work_teacher AS w_teacher FROM work
  WITH READ ONLY;
--
-- EXERCISE_NAME_IDX
--
  CREATE INDEX "EXERCISE_NAME_IDX" ON "EXERCISE" ("EXERCISE_NAME");
--
-- TEACHER_EMAIL_IDX
--
  CREATE UNIQUE INDEX "TEACHER_EMAIL_IDX" ON "TEACHER" ("TEACHER_EMAIL");
--
-- EXERCISE_TEACHER_FK
--
ALTER TABLE "EXERCISE" ADD CONSTRAINT "EXERCISE_TEACHER_FK" FOREIGN KEY ("EXERCISE_TEACHER") REFERENCES "TEACHER"("TEACHER_AGREEMENTID") ON DELETE CASCADE ENABLE;

--
-- WORK_ASSIGNMENT_FK1
--
ALTER TABLE "WORK_ASSIGNMENT" ADD CONSTRAINT "WORK_ASSIGNMENT_FK1" FOREIGN KEY ("FK_EXERCISE_NAME","FK_SUBJECT","FK_TEACHER","FK_THEME","FK_WORK_NAME") REFERENCES "WORK_EXERCISE"("FK_EXERCISE_NAME","FK_SUBJECT","FK_TEACHER","FK_THEME","FK_WORK_NAME") ENABLE;

--
-- WORK_ASSIGNMENT_FK2
--
ALTER TABLE "WORK_ASSIGNMENT" ADD CONSTRAINT "WORK_ASSIGNMENT_FK2" FOREIGN KEY ("FK_STUDENTNUM") REFERENCES "STUDENT"("STUDENT_STUDENTNUMBER") ON DELETE CASCADE ENABLE;

--
-- WORK_EXERCISE_EXERCISE_FK1
--
ALTER TABLE "WORK_EXERCISE" ADD CONSTRAINT "WORK_EXERCISE_EXERCISE_FK1" FOREIGN KEY ("FK_EXERCISE_NAME","FK_SUBJECT","FK_THEME","FK_TEACHER") REFERENCES "EXERCISE"("EXERCISE_NAME","EXERCISE_SUBJECT","EXERCISE_THEME","EXERCISE_TEACHER") ENABLE;

--
-- WORK_EXERCISE_WORK_FK1
--
ALTER TABLE "WORK_EXERCISE" ADD CONSTRAINT "WORK_EXERCISE_WORK_FK1" FOREIGN KEY ("FK_WORK_NAME","FK_SUBJECT","FK_THEME","FK_TEACHER") REFERENCES "WORK"("WORK_NAME","WORK_SUBJECT","WORK_THEME","WORK_TEACHER") ON DELETE CASCADE ENABLE;

