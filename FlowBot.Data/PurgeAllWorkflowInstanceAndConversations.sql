USE [FlowBot]
GO
DELETE FROM [dbo].[Bookmarks]
GO
DELETE FROM [dbo].[ExternalTasks]
GO
DELETE FROM [dbo].[Messages]
GO
DELETE FROM [dbo].[Conversations]
GO
DELETE FROM [dbo].[WorkflowInstances]
GO

USE [FlowBotWorkflowInstance]
GO
DELETE FROM [System.Activities.DurableInstancing].[IdentityOwnerTable]
GO
DELETE FROM [System.Activities.DurableInstancing].[InstanceMetadataChangesTable]
GO
DELETE FROM [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
GO
DELETE FROM [System.Activities.DurableInstancing].[KeysTable]
GO
DELETE FROM [System.Activities.DurableInstancing].[LockOwnersTable]
GO
DELETE FROM [System.Activities.DurableInstancing].[RunnableInstancesTable]
GO
DELETE FROM [System.Activities.DurableInstancing].[InstancesTable]
GO
