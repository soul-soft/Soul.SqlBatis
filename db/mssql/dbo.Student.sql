CREATE TABLE [dbo].[Student] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Name]         NCHAR (10)      NULL,
    [FirstName]    VARCHAR (50)    NULL,
    [Address]      CHAR (100)      NULL,
    [CreationTime] DATE            NULL,
    [State]        INT             NULL,
    [Money]        DECIMAL (18, 2) NULL,
    [RowVersion]   VARCHAR (50)    NULL,
    [Age]          INT             NULL,
    [BinaryData]   VARBINARY(50)     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

