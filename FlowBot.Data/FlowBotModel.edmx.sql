
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/02/2016 11:32:40
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

IF OBJECT_ID(N'[dbo].[FK_UserGroupUser_UserGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserGroupUser] DROP CONSTRAINT [FK_UserGroupUser_UserGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_UserGroupUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserGroupUser] DROP CONSTRAINT [FK_UserGroupUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserGroupExternalTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExternalTasks] DROP CONSTRAINT [FK_UserGroupExternalTask];
GO
IF OBJECT_ID(N'[dbo].[FK_ConversationMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_ConversationMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMessageFrom]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_UserMessageFrom];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMessageTo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_UserMessageTo];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkflowInstanceBookmark]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Bookmarks] DROP CONSTRAINT [FK_WorkflowInstanceBookmark];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkflowWorkflowInstance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkflowInstances] DROP CONSTRAINT [FK_WorkflowWorkflowInstance];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkflowInstanceConversation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conversations] DROP CONSTRAINT [FK_WorkflowInstanceConversation];
GO
IF OBJECT_ID(N'[dbo].[FK_UserExternalTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExternalTasks] DROP CONSTRAINT [FK_UserExternalTask];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ExternalTasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExternalTasks];
GO
IF OBJECT_ID(N'[dbo].[Workflows]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Workflows];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[UserGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserGroups];
GO
IF OBJECT_ID(N'[dbo].[Conversations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Conversations];
GO
IF OBJECT_ID(N'[dbo].[Messages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Messages];
GO
IF OBJECT_ID(N'[dbo].[Bookmarks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Bookmarks];
GO
IF OBJECT_ID(N'[dbo].[WorkflowInstances]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkflowInstances];
GO
IF OBJECT_ID(N'[dbo].[UserGroupUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserGroupUser];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ExternalTasks'
CREATE TABLE [dbo].[ExternalTasks] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [ClaimDate] datetime  NULL,
    [CompletionDate] datetime  NULL,
    [UserGroup_Id] uniqueidentifier  NOT NULL,
    [Worker_Id] uniqueidentifier  NULL
);
GO

-- Creating table 'Workflows'
CREATE TABLE [dbo].[Workflows] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [Package] nvarchar(64)  NOT NULL,
    [Name] nvarchar(64)  NOT NULL,
    [Body] nvarchar(max)  NOT NULL,
    [Major] int  NOT NULL,
    [Minor] int  NOT NULL,
    [Build] int  NOT NULL,
    [Revision] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [FirstName] nvarchar(64)  NOT NULL,
    [LastName] nvarchar(64)  NOT NULL
);
GO

-- Creating table 'UserGroups'
CREATE TABLE [dbo].[UserGroups] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [Name] nvarchar(64)  NOT NULL
);
GO

-- Creating table 'Conversations'
CREATE TABLE [dbo].[Conversations] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [ExternalId] nvarchar(max)  NULL,
    [WorkflowInstance_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [Body] nvarchar(max)  NOT NULL,
    [Conversation_Id] uniqueidentifier  NOT NULL,
    [From_Id] uniqueidentifier  NOT NULL,
    [To_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Bookmarks'
CREATE TABLE [dbo].[Bookmarks] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [Name] nvarchar(64)  NOT NULL,
    [OwnerDisplayName] nvarchar(64)  NOT NULL,
    [CompletionDate] datetime  NULL,
    [WorkflowInstance_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'WorkflowInstances'
CREATE TABLE [dbo].[WorkflowInstances] (
    [Id] uniqueidentifier  NOT NULL,
    [CreateDate] datetime  NOT NULL,
    [InstanceId] uniqueidentifier  NOT NULL,
    [ExternalId] nvarchar(64)  NULL,
    [CompletionDate] datetime  NULL,
    [Workflow_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserGroupUser'
CREATE TABLE [dbo].[UserGroupUser] (
    [UserGroups_Id] uniqueidentifier  NOT NULL,
    [Users_Id] uniqueidentifier  NOT NULL
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

-- Creating primary key on [Id] in table 'Bookmarks'
ALTER TABLE [dbo].[Bookmarks]
ADD CONSTRAINT [PK_Bookmarks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkflowInstances'
ALTER TABLE [dbo].[WorkflowInstances]
ADD CONSTRAINT [PK_WorkflowInstances]
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

-- Creating foreign key on [WorkflowInstance_Id] in table 'Bookmarks'
ALTER TABLE [dbo].[Bookmarks]
ADD CONSTRAINT [FK_WorkflowInstanceBookmark]
    FOREIGN KEY ([WorkflowInstance_Id])
    REFERENCES [dbo].[WorkflowInstances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkflowInstanceBookmark'
CREATE INDEX [IX_FK_WorkflowInstanceBookmark]
ON [dbo].[Bookmarks]
    ([WorkflowInstance_Id]);
GO

-- Creating foreign key on [Workflow_Id] in table 'WorkflowInstances'
ALTER TABLE [dbo].[WorkflowInstances]
ADD CONSTRAINT [FK_WorkflowWorkflowInstance]
    FOREIGN KEY ([Workflow_Id])
    REFERENCES [dbo].[Workflows]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkflowWorkflowInstance'
CREATE INDEX [IX_FK_WorkflowWorkflowInstance]
ON [dbo].[WorkflowInstances]
    ([Workflow_Id]);
GO

-- Creating foreign key on [WorkflowInstance_Id] in table 'Conversations'
ALTER TABLE [dbo].[Conversations]
ADD CONSTRAINT [FK_WorkflowInstanceConversation]
    FOREIGN KEY ([WorkflowInstance_Id])
    REFERENCES [dbo].[WorkflowInstances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkflowInstanceConversation'
CREATE INDEX [IX_FK_WorkflowInstanceConversation]
ON [dbo].[Conversations]
    ([WorkflowInstance_Id]);
GO

-- Creating foreign key on [Worker_Id] in table 'ExternalTasks'
ALTER TABLE [dbo].[ExternalTasks]
ADD CONSTRAINT [FK_UserExternalTask]
    FOREIGN KEY ([Worker_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserExternalTask'
CREATE INDEX [IX_FK_UserExternalTask]
ON [dbo].[ExternalTasks]
    ([Worker_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------