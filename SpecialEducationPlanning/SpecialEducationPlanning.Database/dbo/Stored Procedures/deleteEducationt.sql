CREATE PROCEDURE [dbo].[deleteAiep](@Aiep AS VARCHAR(max))
AS
BEGIN

Create Table #builderidtable (
    builderid int
);

insert into #builderidtable
select builderid from [dbo].[BuilderEducationerAiep] bdd
inner join [dbo].[Aiep] d
on bdd.Aiepid = d.id
and d.Aiepcode = @Aiep
where builderid IN( select builderid from [dbo].[BuilderEducationerAiep] bdd
group by builderid
having count(1) = 1)

Create Table #enduseridtable1 (
    enduserid int
);

insert into #enduseridtable1
select enduserid from [dbo].[EndUserAiep] eud
inner join [dbo].[Aiep] d
on eud.Aiepid = d.id
and d.Aiepcode = @Aiep
where enduserid IN
(select enduserid from [dbo].[EndUserAiep] eud
group by enduserid
having count(1) = 1)


delete from [dbo].[RomItem] 
where exists 
(select * from [dbo].[Version] v
inner join  [dbo].[Plan] p
on v.planid = p.id
inner join [dbo].[Project] pj
on p.[ProjectId] = pj.id
inner join [dbo].[Aiep] d
on pj.Aiepid = d.id
and d.Aiepcode = @Aiep
where v.id = [RomItem].VersionId)


update  [dbo].[Plan] 
set MasterVersionId =  NULL
where exists
(select  *  from [dbo].[Project] pj
inner join [dbo].[Aiep]  d
on d.id = pj.Aiepid
where d.Aiepcode = @Aiep
and  pj.id = [Plan].projectid);

Delete from [dbo].[Version] 
where exists
(Select * from  [dbo].[Plan] p
inner join [dbo].[Project] pj
on p.[ProjectId] = pj.id
inner join [dbo].[Aiep] d
on pj.Aiepid = d.id
and d.Aiepcode = @Aiep
where [Version].planid = p.id)

delete from [dbo].[Plan] 
where exists
(select  *  from [dbo].[Project] pj
inner join [dbo].[Aiep]  d
on d.id = pj.Aiepid
where d.Aiepcode = @Aiep
and  pj.id = [Plan].projectid);

Delete from [dbo].[Project]
where exists (select * from [dbo].[Aiep] d
where d.Aiepcode = @Aiep
and [Project].Aiepid = d.id);


Delete from [dbo].[BuilderEducationerAiep] 
where exists (select * from [dbo].[Aiep] d
where d.Aiepcode = @Aiep
and  [BuilderEducationerAiep].Aiepid = d.id);

Delete from [dbo].[EndUserAiep] 
where exists(select id from [dbo].[Aiep] d
where d.Aiepcode = @Aiep
and [EndUserAiep].Aiepid = d.id);


delete from [dbo].[EndUser]
where id in (Select enduserid from #enduseridtable1)

delete from [dbo].[Builder]
where id in (select builderid from #builderidtable)


END;

