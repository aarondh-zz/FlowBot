
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/29/2016 08:54:29
-- Generated from EDMX file: C:\Users\v-adai\Documents\Visual Studio 2015\Projects\FlowBot\FlowBot.Data\FlowBotModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Flowbot];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ExternalTasks'
CREATE TABLE [dbo].[ExternalTasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserGroup_Id] int  NOT NULL
);
GO

-- Creating table 'Workflows'
CREATE TABLE [dbo].[Workflows] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Version] nvarchar(max)  NOT NULL,
    [Body] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserGroups'
CREATE TABLE [dbo].[UserGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Conversations'
CREATE TABLE [dbo].[Conversations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StartDate] nvarchar(max)  NOT NULL,
    [EndDate] nvarchar(max)  NULL
);
GO

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Body] nvarchar(max)  NOT NULL,
    [Conversation_Id] int  NOT NULL,
    [From_Id] int  NOT NULL,
    [To_Id] int  NOT NULL
);
GO

-- Creating table 'UserGroupUser'
CREATE TABLE [dbo].[UserGroupUser] (
    [UserGroups_Id] int  NOT NULL,
    [Users_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'ExternalTasks'
ALTER TABLE [dbo].[ExternalTasks]
ADD CONSTRAINT [PK_ExternalTasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Workflows'
ALTER TABLE [dbo].[Workflows]
ADD CONSTRAINT [PK_Workflows]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserGroups'
ALTER TABLE [dbo].[UserGroups]
ADD CONSTRAINT [PK_UserGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [PK_Conversations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [PK_Messages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [UserGroups_Id], [Users_Id] in table 'UserGroupUser'
ALTER TABLE [dbo].[UserGroupUser]
ADD CONSTRAINT [PK_UserGroupUser]
    PRIMARY KEY CLUSTERED ([UserGroups_Id], [Users_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserGroups_Id] in table 'UserGroupUser'
ALTER TABLE [dbo].[UserGroupUser]
ADD CONSTRAINT [FK_UserGroupUser_UserGroup]
    FOREIGN KEY ([UserGroups_Id])
    REFERENCES [dbo].[UserGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Users_Id] in table 'UserGroupUser'
ALTER TABLE [dbo].[UserGroupUser]
ADD CONSTRAINT [FK_UserGroupUser_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserGroupUser_User'
CREATE INDEX [IX_FK_UserGroupUser_User]
ON [dbo].[UserGroupUser]
    ([Users_Id]);
GO

-- Creating foreign key on [UserGroup_Id] in table 'ExternalTasks'
ALTER TABLE [dbo].[ExternalTasks]
ADD CONSTRAINT [FK_UserGroupExternalTask]
    FOREIGN KEY ([UserGroup_Id])
    REFERENCES [dbo].[UserGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserGroupExternalTask'
CREATE INDEX [IX_FK_UserGroupExternalTask]
ON [dbo].[ExternalTasks]
    ([UserGroup_Id]);
GO

-- Creating foreign key on [Conversation_Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_ConversationMessage]
    FOREIGN KEY ([Conversation_Id])
    REFERENCES [dbo].[Conversations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConversationMessage'
CREATE INDEX [IX_FK_ConversationMessage]
ON [dbo].[Messages]
    ([Conversation_Id]);
GO

-- Creating foreign key on [From_Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_UserMessageFrom]
    FOREIGN KEY ([From_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMessageFrom'
CREATE INDEX [IX_FK_UserMessageFrom]
ON [dbo].[Messages]
    ([From_Id]);
GO

-- Creating foreign key on [To_Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_UserMessageTo]
    FOREIGN KEY ([To_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMessageTo'
CREATE INDEX [IX_FK_UserMessageTo]
ON [dbo].[Messages]
    ([To_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------