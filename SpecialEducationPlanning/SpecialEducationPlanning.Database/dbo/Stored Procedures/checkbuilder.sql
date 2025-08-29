CREATE PROCEDURE [dbo].[CheckBuilder]
AS
BEGIN


Declare @Buildercount int = (select count(b.id) from [dbo].[Builder] b 
inner join [dbo].[Plan] p 
on p.builderid = b.id
inner join  [dbo].[Project] pj
on p.[ProjectId] = pj.id
inner join [dbo].[Aiep] d
on d.id = pj.AiepId
where b.id not in (
select  bdd.BuilderId from  [dbo].[BuilderEducationerAiep] bdd
where d.id = bdd.Aiepid))

IF @Buildercount > 0
    BEGIN  
 
select distinct(b.id) as builderid,d.id as Aiepid, 'Error' as  Builder_Error from [dbo].[Builder] b 
inner join [dbo].[Plan] p 
on p.builderid = b.id
inner join  [dbo].[Project] pj
on p.[ProjectId] = pj.id
inner join [dbo].[Aiep] d
on d.id = pj.AiepId
where b.id not in (
select  bdd.BuilderId from  [dbo].[BuilderEducationerAiep] bdd
where d.id = bdd.Aiepid)
	END
ELSE
	BEGIN
		PRINT 'Builder Ok' 		
END;

END


