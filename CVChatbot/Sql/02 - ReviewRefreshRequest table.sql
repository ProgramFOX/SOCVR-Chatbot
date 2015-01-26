create table ReviewRefreshRequest
(
	Id int not null primary key identity,
	RegisteredUserId int not null foreign key references RegisteredUser(Id)
)