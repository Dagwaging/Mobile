USE [BannerData]
GO

-- User table creation

/****** Object:  Table [dbo].[User]    Script Date: 04/10/2012 19:58:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[User](
	[switch] [bit] NOT NULL,
	[id] [int] NOT NULL,
	[username] [varchar](20) NOT NULL,
	[email] [varchar](50) NULL,
	[cm] [int] NULL,
	[majors] [varchar](50) NULL,
	[class] [char](2) NULL,
	[year] [char](2) NULL,
	[advisor] [int] NULL,
	[lastname] [nvarchar](50) NOT NULL,
	[firstname] [nvarchar](50) NOT NULL,
	[middlename] [nvarchar](50) NULL,
	[department] [varchar](50) NULL,
	[telephone] [varchar](50) NULL,
	[office] [varchar](50) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[switch] ASC,
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_User] FOREIGN KEY([switch], [advisor])
REFERENCES [dbo].[User] ([switch], [id])
GO

ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_User]
GO

/****** Object:  Index [Index_User_Nonclustered]    Script Date: 04/10/2012 19:59:29 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Index_User_Nonclustered] ON [dbo].[User] 
(
	[switch] ASC,
	[username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- Course table creation

/****** Object:  Table [dbo].[Course]    Script Date: 04/10/2012 20:00:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Course](
	[switch] [bit] NOT NULL,
	[term] [int] NOT NULL,
	[crn] [int] NOT NULL,
	[course] [varchar](50) NOT NULL,
	[title] [varchar](100) NOT NULL,
	[instructor] [int] NULL,
	[credits] [int] NOT NULL,
	[finalday] [char](1) NULL,
	[finalhour] [int] NULL,
	[finalroom] [varchar](50) NULL,
	[enrolled] [int] NOT NULL,
	[maxenrolled] [int] NOT NULL,
	[comments] [varchar](500) NULL,
 CONSTRAINT [PK_Course_1] PRIMARY KEY CLUSTERED 
(
	[switch] ASC,
	[term] ASC,
	[crn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_User] FOREIGN KEY([switch], [instructor])
REFERENCES [dbo].[User] ([switch], [id])
GO

ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_User]
GO

/****** Object:  Index [Index_Course_Nonclustered]    Script Date: 04/18/2012 14:18:56 ******/
CREATE NONCLUSTERED INDEX [Index_Course_Nonclustered] ON [dbo].[Course] 
(
	[switch] ASC,
	[instructor] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- CourseSchedule table creation

/****** Object:  Table [dbo].[CourseSchedule]    Script Date: 04/10/2012 20:00:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CourseSchedule](
	[switch] [bit] NOT NULL,
	[term] [int] NOT NULL,
	[crn] [int] NOT NULL,
	[day] [char](1) NOT NULL,
	[startperiod] [int] NOT NULL,
	[endperiod] [int] NOT NULL,
	[room] [varchar](50) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CourseSchedule]  WITH CHECK ADD  CONSTRAINT [FK_CourseSchedule_Course] FOREIGN KEY([switch], [term], [crn])
REFERENCES [dbo].[Course] ([switch], [term], [crn])
GO

ALTER TABLE [dbo].[CourseSchedule] CHECK CONSTRAINT [FK_CourseSchedule_Course]
GO

/****** Object:  Index [Index_CourseSchedule_Nonclustered]    Script Date: 04/10/2012 20:01:16 ******/
CREATE NONCLUSTERED INDEX [Index_CourseSchedule_Nonclustered] ON [dbo].[CourseSchedule] 
(
	[switch] ASC,
	[room] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [Index_CourseSchedule]    Script Date: 04/10/2012 20:11:33 ******/
CREATE CLUSTERED INDEX [Index_CourseSchedule] ON [dbo].[CourseSchedule] 
(
	[switch] ASC,
	[term] ASC,
	[crn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- Enrollment table creation

/****** Object:  Table [dbo].[Enrollment]    Script Date: 04/10/2012 20:01:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Enrollment](
	[switch] [bit] NOT NULL,
	[student] [int] NOT NULL,
	[term] [int] NOT NULL,
	[crn] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Enrollment]  WITH CHECK ADD  CONSTRAINT [FK_Enrollment_Course] FOREIGN KEY([switch], [term], [crn])
REFERENCES [dbo].[Course] ([switch], [term], [crn])
GO

ALTER TABLE [dbo].[Enrollment] CHECK CONSTRAINT [FK_Enrollment_Course]
GO

ALTER TABLE [dbo].[Enrollment]  WITH CHECK ADD  CONSTRAINT [FK_Enrollment_User] FOREIGN KEY([switch], [student])
REFERENCES [dbo].[User] ([switch], [id])
GO

ALTER TABLE [dbo].[Enrollment] CHECK CONSTRAINT [FK_Enrollment_User]
GO

/****** Object:  Index [Index_Enrollment_Nonclustered]    Script Date: 04/10/2012 20:01:50 ******/
CREATE NONCLUSTERED INDEX [Index_Enrollment_Nonclustered] ON [dbo].[Enrollment] 
(
	[switch] ASC,
	[term] ASC,
	[crn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [Index_Enrollment]    Script Date: 04/10/2012 20:12:16 ******/
CREATE CLUSTERED INDEX [Index_Enrollment] ON [dbo].[Enrollment] 
(
	[switch] ASC,
	[student] ASC,
	[term] ASC,
	[crn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- Stored procedure creation

/****** Object:  StoredProcedure [dbo].[spGetTerms]    Script Date: 04/18/2012 13:55:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetTerms]
	@switch bit
AS
BEGIN
	SELECT DISTINCT term
	FROM Course
	WHERE switch = @switch
	ORDER BY term
END

GO

/****** Object:  StoredProcedure [dbo].[spDeleteData]    Script Date: 04/10/2012 20:03:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteData]
	@switch bit
AS
BEGIN
	DELETE FROM Enrollment WHERE switch = @switch
	DELETE FROM CourseSchedule WHERE switch = @switch
	DELETE FROM Course WHERE switch = @switch
	DELETE FROM [User] WHERE switch = @switch
END

GO

/****** Object:  StoredProcedure [dbo].[spGetCourse]    Script Date: 04/10/2012 20:04:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCourse]
	@switch bit,
	@term int,
	@crn int
AS
BEGIN
	DECLARE @instructor varchar(20)
	SELECT @instructor = u.username
	FROM [Course] c
		JOIN [User] u ON u.switch = @switch AND c.instructor = u.id
	WHERE c.switch = @switch
		AND c.term = @term
		AND c.crn = @crn
		
	SELECT term, crn, course, title, credits, finalday, finalhour, finalroom, enrolled, maxenrolled, comments, @instructor AS instructor
	FROM [Course] c
	WHERE c.switch = @switch
		AND c.term = @term
		AND c.crn = @crn
END

GO

/****** Object:  StoredProcedure [dbo].[spGetCourseEnrollment]    Script Date: 04/10/2012 20:04:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCourseEnrollment]
	@switch bit,
	@term int,
	@crn int
AS
BEGIN
	SELECT u.username
	FROM [Enrollment] e
		JOIN [User] u ON u.switch = @switch AND u.id = e.student
	WHERE e.switch = @switch
		AND e.term = @term
		AND e.crn = @crn
END

GO

/****** Object:  StoredProcedure [dbo].[spGetCourseSchedule]    Script Date: 04/10/2012 20:04:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCourseSchedule]
	@switch bit,
	@term int,
	@crn int
AS
BEGIN
	SELECT [day], startperiod, endperiod, room
	FROM CourseSchedule
	WHERE switch = @switch
		AND term = @term
		AND crn = @crn
END

GO

/****** Object:  StoredProcedure [dbo].[spGetInstructorSchedule]    Script Date: 04/23/2012 13:26:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetInstructorSchedule]
	@switch bit,
	@term int,
	@username varchar(50)
AS
BEGIN
	SELECT c.term, c.crn
	FROM [User] u
		JOIN [Course] c ON c.switch = @switch AND c.instructor = u.id AND c.term = @term
	WHERE u.switch = @switch
		AND u.username = @username
END

GO

/****** Object:  StoredProcedure [dbo].[spGetRoomSchedule]    Script Date: 04/23/2012 13:28:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetRoomSchedule]
	@switch bit,
	@term int,
	@room varchar(50)
AS
BEGIN
	SELECT term, crn, [day], startperiod, endperiod
	FROM CourseSchedule
	WHERE switch = @switch
		AND term = @term
		AND room = @room
END

GO

/****** Object:  StoredProcedure [dbo].[spGetUser]    Script Date: 04/10/2012 20:04:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetUser]
	@switch bit,
	@username varchar(50)
AS
BEGIN
	DECLARE @advisor varchar(20)
	SELECT @advisor = a.username
	FROM [User] u
		JOIN [User] a ON a.switch = @switch AND u.advisor = a.id
	WHERE u.switch = @switch
		AND u.username = @username

	SELECT username, email, cm, majors, class, [year], lastname, firstname, middlename, department, telephone, office, @advisor AS advisor
	FROM [User]
	WHERE switch = @switch
		AND username = @username
END

GO

/****** Object:  StoredProcedure [dbo].[spGetUserEnrollment]    Script Date: 04/23/2012 13:31:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetUserEnrollment]
	@switch bit,
	@term int,
	@username varchar(50)
AS
BEGIN
	SELECT e.term, e.crn
	FROM [User] u
		JOIN [Enrollment] e ON e.switch = @switch AND e.student = u.id AND e.term = @term
	WHERE u.switch = @switch
		AND u.username = @username
END

GO

/****** Object:  StoredProcedure [dbo].[spInsertCourse]    Script Date: 04/10/2012 20:05:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertCourse]
	@switch bit,
	@term int,
	@crn int,
	@course varchar(50),
	@title varchar(50),
	@instructor varchar(50),
	@credits int,
	@finalday char(1),
	@finalhour int,
	@finalroom varchar(50),
	@enrolled int,
	@maxenrolled int,
	@comments varchar(500)
AS
BEGIN
	DECLARE @instructorid int
	IF @instructor IS NULL
		SET @instructorid = NULL
	ELSE
		SELECT @instructorid = id
		FROM [User]
		WHERE switch = @switch
			AND username = @instructor

	INSERT INTO Course(switch, term, crn, course, title, instructor, credits, finalday, finalhour, finalroom, enrolled, maxenrolled, comments)
	VALUES(@switch, @term, @crn, @course, @title, @instructorid, @credits, @finalday, @finalhour, @finalroom, @enrolled, @maxenrolled, @comments)
END

GO

/****** Object:  StoredProcedure [dbo].[spInsertCourseSchedule]    Script Date: 04/10/2012 20:05:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertCourseSchedule]
	@switch bit,
	@term int,
	@crn int,
	@day char(1),
	@startperiod int,
	@endperiod int,
	@room varchar(50)
AS
BEGIN
	INSERT INTO CourseSchedule(switch, term, crn, [day], startperiod, endperiod, room)
	VALUES(@switch, @term, @crn, @day, @startperiod, @endperiod, @room)
END

GO

/****** Object:  StoredProcedure [dbo].[spInsertEnrollment]    Script Date: 04/10/2012 20:05:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertEnrollment]
	@switch bit,
	@student varchar(50),
	@term int,
	@crn int
AS
BEGIN
	DECLARE @studentid int
	SELECT @studentid = id
	FROM [User]
	WHERE switch = @switch
		AND username = @student

	INSERT INTO Enrollment(switch, student, term, crn)
	VALUES(@switch, @studentid, @term, @crn)
END

GO

/****** Object:  StoredProcedure [dbo].[spInsertUser]    Script Date: 04/10/2012 20:05:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertUser]
	@switch bit,
	@username varchar(50),
	@email varchar(50),
	@cm int,
	@majors varchar(50),
	@class char(2),
	@year char(2),
	@lastname nvarchar(50),
	@firstname nvarchar(50),
	@middlename nvarchar(50),
	@department varchar(50),
	@telephone varchar(50),
	@office varchar(50)
AS
BEGIN
	DECLARE @id int
	SELECT @id = ISNULL(MAX(id), 0) + 1
	FROM [User]
	WHERE switch = @switch

	INSERT INTO [User](switch, id, username, email, cm, majors, class, [year], lastname, firstname, middlename, department, telephone, office)
	VALUES(@switch, @id, @username, @email, @cm, @majors, @class, @year, @lastname, @firstname, @middlename, @department, @telephone, @office)
END

GO

/****** Object:  StoredProcedure [dbo].[spSearchCourses]    Script Date: 04/23/2012 13:33:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSearchCourses]
	@switch bit,
	@term int,
	@search varchar(50)
AS
BEGIN
	SELECT term, crn, course, title, credits, finalday, finalhour, finalroom, enrolled, maxenrolled, comments, u.username AS instructor
	FROM [Course] c
		LEFT JOIN [User] u ON u.switch = @switch AND c.instructor = u.id
	WHERE c.switch = @switch
		AND c.term = @term
		AND (c.course LIKE '%' + @search + '%'
			OR c.title LIKE '%' + @search + '%')
END

GO

/****** Object:  StoredProcedure [dbo].[spSearchUsers]    Script Date: 04/10/2012 20:06:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSearchUsers]
	@switch bit,
	@search varchar(50)
AS
BEGIN
	SELECT u.username, u.email, u.cm, u.majors, u.class, u.[year], u.lastname, u.firstname, u.middlename, u.department, u.telephone, u.office, a.username AS advisor
	FROM [User] u
		LEFT JOIN [User] a ON a.switch = @switch AND u.advisor = a.id
	WHERE u.switch = @switch
		AND (u.username LIKE '%' + @search + '%'
			OR u.lastname LIKE '%' + @search + '%'
			OR u.firstname LIKE '%' + @search + '%'
			OR u.middlename LIKE '%' + @search + '%')
END

GO

/****** Object:  StoredProcedure [dbo].[spSetAdvisor]    Script Date: 04/10/2012 20:07:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSetAdvisor]
	@switch bit,
	@student varchar(50),
	@advisor varchar(50)
AS
BEGIN
	DECLARE @advisorid int
	SELECT @advisorid = id
	FROM [User]
	WHERE switch = @switch
		AND username = @advisor
		
	UPDATE [User]
	SET advisor = @advisorid
	WHERE switch = @switch
		AND username = @student
END

GO

