CREATE TABLE [dbo].[HousingSpecificationTemplates] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [HousingSpecificationId]    INT            NOT NULL,
    [PlanId]                    INT            NOT NULL,
    CONSTRAINT [PK_HousingSpecificationTemplates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HousingSpecificationTemplates_HousingSpecification_HousingSpecificationId] FOREIGN KEY ([HousingSpecificationId]) REFERENCES [dbo].[HousingSpecification] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HousingSpecificationTemplates_Plan_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plan] ([Id]) ON DELETE NO ACTION
);

GO