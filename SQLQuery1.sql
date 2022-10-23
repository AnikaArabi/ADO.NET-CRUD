CREATE TABLE teachers
(
	teacherid INT PRIMARY KEY,
	[name] NVARCHAR(40) NOT NULL,
	joindate DATE NOT NULL,
	picture NVARCHAR(150) NOT NULL,
	post NVARCHAR(30) NOT NULL,
	basicsalary MONEY NOT NULL
)
CREATE TABLE qualifications
(
	qualificationid INT PRIMARY KEY,
	degree NVARCHAR(30) NOT NULL,
	institute NVARCHAR(50) NOT NULL,
	result NVARCHAR(20) NOT NULL,
	passingyear INT NOT NULL,
	teacherid INT NOT NULL REFERENCES teachers(teacherid)
)
GO