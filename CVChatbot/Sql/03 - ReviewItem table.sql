create table ReviewItem
(
	Id int not null primary key identity,
	ReviewId int not null,
	RegisteredUserId int not null foreign key references RegisteredUser(Id),
	AuditPassed bit null,
	PostTag1 nvarchar(max) null,
	PostTag2 nvarchar(max) null,
	PostTag3 nvarchar(max) null,
	PostTag4 nvarchar(max) null,
	PostTag5 nvarchar(max) null
)