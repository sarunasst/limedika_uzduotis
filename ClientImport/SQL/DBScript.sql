﻿USE [master]
GO
/****** Object:  Database [ClientDB]    Script Date: 2023-03-03 11:19:27 ******/
CREATE DATABASE [ClientDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ClientDB', FILENAME = N'C:\Users\NEXUS\ClientDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ClientDB_log', FILENAME = N'C:\Users\NEXUS\ClientDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [ClientDB] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ClientDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ClientDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ClientDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ClientDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ClientDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ClientDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [ClientDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [ClientDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ClientDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ClientDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ClientDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ClientDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ClientDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ClientDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ClientDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ClientDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ClientDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ClientDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ClientDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ClientDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ClientDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ClientDB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [ClientDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ClientDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ClientDB] SET  MULTI_USER 
GO
ALTER DATABASE [ClientDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ClientDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ClientDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ClientDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ClientDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ClientDB] SET QUERY_STORE = OFF
GO
USE [ClientDB]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [ClientDB]
GO

/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2023-03-03 11:19:28 ******/
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Clients]    Script Date: 2023-03-03 11:19:28 ******/
CREATE TABLE [dbo].[Clients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](255) NOT NULL,
	[PostCode] [nvarchar](31) NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Log]    Script Date: 2023-03-03 11:19:28 ******/
CREATE TABLE [dbo].[Log](
	[Time] [datetime] NOT NULL,
	[Event] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_ClientName]    Script Date: 2023-03-03 11:19:28 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ClientName] ON [dbo].[Clients]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Trigger [TRG_ClientUpdated]    Script Date: 2023-03-03 11:19:28 ******/
CREATE TRIGGER TRG_ClientUpdated
ON dbo.Clients AFTER UPDATE 
AS BEGIN
      INSERT INTO 
          dbo.Log(Time, Event)
          SELECT
             GETDATE(), 'Pakestas esamas klientas. Iš: [' + CAST(d.Id AS VARCHAR(11)) + ', ' + d.Name + ', ' + d.Address + ', ' + ISNULL(d.PostCode, 'null') + '] į: [' + CAST(i.Id AS VARCHAR(11)) + ', ' + i.Name + ', ' + i.Address + ', ' + ISNULL(i.PostCode, 'null') + ']'
          FROM 
             DELETED d
			 full join 
			 INSERTED i on d.Id = i.Id
END
GO

/****** Object:  Trigger [TRG_ClientInserted]    Script Date: 2023-03-03 11:19:28 ******/
CREATE TRIGGER TRG_ClientInserted
ON dbo.Clients AFTER INSERT 
AS BEGIN
      INSERT INTO 
          dbo.Log(Time, Event)
          SELECT
             GETDATE(), 'Pridėtas naujas klientas: [' + CAST(i.Id AS VARCHAR(11)) + ', ' + i.Name + ', ' + i.Address + ', ' + ISNULL(i.PostCode, 'null') + ']'
          FROM 
             INSERTED i
END
GO