1. users can login using gmail or anytoher means

what users can do

1. They can upload their important files, photos, scan details with headers (access should be given to such user) payment should be made - Free trail for 30 days
2. They can set reminder(for appointment) - Free trail for 30 days
3. Monthly expense tracker - Free trail for 30 days
4. User Management page for admins
5. Add tasks for the day and track them delete them,update them
6. Monitor Gold Rates today in India as per their respective State



after that per month 5 rupees they should pay to make use of it
{
  "firstName": "Karthik",
  "surName": "Kannan",
  "emailId": "Karthik24810@hotmail.com",
  "mobileNumber": "8056284845",
  "displayName": "Karthik Kannan",
  "userRoleId": 2
}

CREATE TABLE UserRole (
    UserRoleID INT IDENTITY(1,1) PRIMARY KEY, -- Primary key, auto-incremented
    RoleName  Varchar(100) not null,
    CreatedAt DATETIME DEFAULT GETDATE() not null,
    CreatedBy Varchar(500),
	UpdatedAt DATETIME null,
	UpdatedBy Varchar(500) null,
    IsActive BIT null
);

INSERT INTO UserRole (RoleName, CreatedAt, CreatedBy, IsActive)
VALUES 
('Admin', GETDATE(), 'Karthik.Kannan270597@gmail.com', 1),
('Free User', GETDATE(), 'Karthik.Kannan270597@gmail.com', 1),
('Premium User', GETDATE(), 'Karthik.Kannan270597@gmail.com', 1);

CREATE TABLE UserDetail (
    UserId INT IDENTITY(1,1) PRIMARY KEY not null,         -- Primary key, auto-incremented
    FirstName VARCHAR(200) NOT NULL,           -- First name of the user
    SurName VARCHAR(200)  Null,             -- Surname of the user
    EmailID VARCHAR(500) NOT NULL,            -- Email ID of the user
    MobileNumber int NULL,        -- Mobile number of the user
    DisplayName VARCHAR(500) NOT NULL,        -- Display name of the user
	IsActive bit null,
    CreatedAt DATETIME DEFAULT GETDATE() Not null,-- Timestamp for when the user was created
	UpdatedAt DATETIME  null,-- Timestamp for when the user was last updated
	UpdatedBy Varchar(500) null
);
ALTER TABLE UserDetail
ADD CONSTRAINT FK_UserDetail_UserRole FOREIGN KEY (UserRoleId) REFERENCES UserRole(UserRoleID);

Create Table UserRoleMapping(
 Id INT IDENTITY(1,1) PRIMARY KEY not null,
 UserId INT Not null,
 UserRoleID int Not null,
 CreatedAt DATETIME DEFAULT GETDATE() Not null,-- Timestamp for when the user was created
 CreatedBy varchar(500) not null,
 UpdatedAt DATETIME  null,-- Timestamp for when the user was last updated
 UpdatedBy Varchar(500) null
 )

ALTER TABLE UserRoleMapping
ADD CONSTRAINT FK_UserRoleMapping_userId FOREIGN KEY (UserId) REFERENCES UserDetail(UserId);

ALTER TABLE UserRoleMapping
ADD CONSTRAINT FK_UserRoleMapping_UserRoleID FOREIGN KEY (UserRoleID) REFERENCES UserRole(UserRoleID);

Create Table ExpenseTrackerCategory(
CategoryId INT IDENTITY(1,1) PRIMARY KEY not null,
CategoryName varchar(500) Not null,
IsActive bit  null,
CreatedBy Varchar(500) not null,
CreateDate DateTime Default GetDate() not null
);

INSERT INTO ExpenseTrackerCategory (CategoryName, IsActive, CreatedBy)
VALUES 
('Food', 1, 'Karthik.Kannan270597@gmail.com'),
('Transportation', 1, 'Karthik.Kannan270597@gmail.com'),
('Groceries', 1, 'Karthik.Kannan270597@gmail.com'),
('Clothing', 1, 'Karthik.Kannan270597@gmail.com'),
('Membership', 1, 'Karthik.Kannan270597@gmail.com'),
('Taxes', 1, 'Karthik.Kannan270597@gmail.com'),
('Utilities', 1, 'Karthik.Kannan270597@gmail.com'),
('Vacation', 1, 'Karthik.Kannan270597@gmail.com'),
('Childcare', 1, 'Karthik.Kannan270597@gmail.com'),
('Parenting Expenses', 1, 'Karthik.Kannan270597@gmail.com'),
('Entertainment', 1, 'Karthik.Kannan270597@gmail.com'),
('Retirement', 1, 'Karthik.Kannan270597@gmail.com'),
('Debt', 1, 'Karthik.Kannan270597@gmail.com'),
('Savings', 1, 'Karthik.Kannan270597@gmail.com'),
('Miscellaneous', 1, 'Karthik.Kannan270597@gmail.com');


Create Table UserExpenseTracker(
UserExpenseTrackerId int IDENTITY(1,1) PRIMARY KEY not null,
UserId Int Not null,
Description Varchar(500) Not null,
CategoryId Int Not null,
ExpenseDate DateTime Not null,
Price float Not null,
CreatedBy Varchar(500) not null,
CreateDate DateTime Default GetDate() not null,
UpdatedBy varchar(500) null,
UpdateDate DateTime null

);

ALTER TABLE UserExpenseTracker
ADD CONSTRAINT FK_UserExpenseTracker_UserId FOREIGN KEY (UserId) REFERENCES UserDetail(UserId);

ALTER TABLE UserExpenseTracker
ADD CONSTRAINT FK_UserExpenseTracker_CategoryId FOREIGN KEY (CategoryId) REFERENCES ExpenseTrackerCategory(CategoryId);

--save sp--

CREATE Procedure Sp_SaveExpenseData   
 @ExpensesXmlData XML,  
 @UserEmail Varchar(500),  
 @isEdited bit = 0,  
 @Result BIT OUTPUT  
AS  
Begin  
set nocount on;  
BEGIN TRANSACTION;  
BEGIN TRY  
Declare @UserId int;  
select @UserId=UserId from UserDetail where EmailID=@userEmail;  
if(@isEdited=0)  
Begin  
Insert into [dbo].[UserExpenseTracker](UserId,Description,Price,CategoryId,ExpenseDate,CreatedBy)  
SELECT  
        @UserId as UserId,  
        T.Item.value('(Description)[1]', 'NVARCHAR(500)') AS Description,  
        T.Item.value('(Price)[1]', 'DECIMAL(18, 2)') AS Price,  
        T.Item.value('(CategoryId)[1]', 'INT') AS CategoryId,  
        T.Item.value('(ExpenseDate)[1]', 'DATETIME') AS ExpenseDate,  
  @UserEmail as CreatedBy  
    FROM @ExpensesXmlData.nodes('/ArrayOfExpenseTrackerModel/ExpenseTrackerModel') AS T(Item);  
 end  
 else  
 BEGIN  
 SELECT  
        @UserId as UserId,  
        T.Item.value('(Description)[1]', 'NVARCHAR(500)') AS Description,  
        T.Item.value('(Price)[1]', 'DECIMAL(18, 2)') AS Price,  
        T.Item.value('(CategoryId)[1]', 'INT') AS CategoryId,  
        T.Item.value('(ExpenseDate)[1]', 'DATETIME') AS ExpenseDate,  
  T.Item.value('(UserExpenseTrackerId)[1]', 'int') AS UserExpenseTrackerId,  
  @UserEmail as CreatedBy into #tempExpenseTable  
    FROM @ExpensesXmlData.nodes('/ArrayOfExpenseTrackerModel/ExpenseTrackerModel') AS T(Item);  
  
 update UET set UET.CategoryId=TET.CategoryId,UET.Description=TET.Description,UET.ExpenseDate=TET.ExpenseDate,  
 UET.Price=TET.Price  
 from  [dbo].[UserExpenseTracker] UET inner join  #tempExpenseTable TET on UET.UserExpenseTrackerId = TET.UserExpenseTrackerId  
 where UET.UserId=@UserId  
  
 END  
      commit Transaction;  
   SET @Result = 1;  
    END TRY  
 BEGIN CATCH  
  IF @@TRANCOUNT > 0  
        BEGIN  
            ROLLBACK TRANSACTION;  
        END  
        -- Error handling  
        DECLARE @ErrorMessage NVARCHAR(4000);  
        DECLARE @ErrorSeverity INT;  
        DECLARE @ErrorState INT;  
  
        SELECT   
            @ErrorMessage = ERROR_MESSAGE(),  
            @ErrorSeverity = ERROR_SEVERITY(),  
            @ErrorState = ERROR_STATE();  
  
        -- Log the error or return error details  
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  
  
        -- Set failure result  
        SET @Result = 0;  
    END CATCH  
end 
go
--get sp--

CREATE Procedure Sp_GetExpensesData       
@DateFrom Date=null,      
@DateTo Date=null,      
@Duration varchar(500),      
@UserEmail Varchar(500),    
@Skip int,    
@Take int,  
@IsCategoryWise bit  
As      
Begin     
    
BEGIN TRY    
Declare @UserId int;     
select @UserId=UserId from UserDetail where EmailID=@UserEmail;     
    
--IF @DateFrom IS NOT NULL AND @DateTo is not null    
    
     
--Begin    
    
with Cte1 as(    
select userId,Round(sum(Price),3) as TotalExpenses,count(*) as TotalRows from UserExpenseTracker where UserId=@UserId  
 AND ExpenseDate between @DateFrom and @DateTo  
group by userId )    
select UET.UserExpenseTrackerId,UET.Description,UET.CategoryId,ETC.CategoryName,UET.ExpenseDate,UET.Price,    
cte1.TotalRows,  
cte1.TotalExpenses  
from UserExpenseTracker UET  WITH (NOLOCK)  Inner join ExpenseTrackerCategory ETC  WITH (NOLOCK)      
on  UET.CategoryId=ETC.CategoryId      
Inner join  Cte1 WITH (NOLOCK) on Cte1.userId = UET.userId  
where UET.UserId=@UserId  AND UET.ExpenseDate between @DateFrom and @DateTo    
 ORDER BY   
 CASE WHEN @IsCategoryWise = 1  THEN ETC.CategoryName End,  
 UET.ExpenseDate,UET.Description    
                    OFFSET @Skip ROWS    
                    FETCH NEXT @Take ROWS ONLY;    
--End    
    
--Else    
--Begin    
-- with Cte2 as(    
--select userId,Round(sum(Price),3) as TotalExpenses,count(*) as TotalRows from UserExpenseTracker where UserId=@UserId  
--group by userId )     
--select UET.UserExpenseTrackerId,UET.Description,UET.CategoryId,ETC.CategoryName,UET.ExpenseDate,UET.Price,    
--cte2.TotalRows,  
--cte2.TotalExpenses    
--from UserExpenseTracker UET WITH (NOLOCK) Inner join ExpenseTrackerCategory ETC   WITH (NOLOCK)    
--on  UET.CategoryId=ETC.CategoryId      
--inner join Cte2 WITH (NOLOCK) on Cte2.userId = UET.UserId  
--where UET.UserId=@UserId    
-- ORDER BY UET.ExpenseDate,UET.Description   
--                    OFFSET @Skip ROWS    
--                    FETCH NEXT @Take ROWS ONLY;    
--end    
 END TRY    
 BEGIN CATCH    
  IF @@TRANCOUNT > 0    
        BEGIN    
            ROLLBACK TRANSACTION;    
        END    
        -- Error handling    
        DECLARE @ErrorMessage NVARCHAR(4000);    
        DECLARE @ErrorSeverity INT;    
        DECLARE @ErrorState INT;    
    
        SELECT     
            @ErrorMessage = ERROR_MESSAGE(),    
            @ErrorSeverity = ERROR_SEVERITY(),    
            @ErrorState = ERROR_STATE();    
    
        -- Log the error or return error details    
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);    
  End Catch    
End     
go

CREATE Procedure Sp_SaveUserData       
@UserId int,    
@EmailId varchar(500),    
@MobileNumber bigint,    
@RoleIds varchar(500),    
@UpdatedBy varchar(500),  
@IsActive bit,  
@Result Bit OUTPUT    
AS      
Begin      
set nocount on;      
BEGIN TRANSACTION;      
BEGIN TRY      
     
    
SELECT value as RoleId into #Roles FROM STRING_SPLIT(@RoleIds, ',');    
    
Update UserDetail set MobileNumber=@MobileNumber, IsActive=@IsActive,UpdatedAt=getdate(),UpdatedBy=@UpdatedBy     
where UserId=@UserId and EmailID=@EmailId    
    
MERGE UserRoleMapping AS TARGET    
    USING #Roles AS SOURCE     
    ON TARGET.UserRoleID = SOURCE.RoleId and Target.UserId = @UserId    
    WHEN NOT MATCHED BY TARGET     
    THEN INSERT (UserId, UserRoleId, CreatedAt,CreatedBy)              
         VALUES (@UserId, SOURCE.RoleId, getdate(),@Updatedby)    
    
    WHEN NOT MATCHED BY SOURCE  and Target.UserId = @UserId   
    THEN DELETE;    
    
    
    
      commit Transaction;      
   SET @Result = 1;      
    END TRY      
 BEGIN CATCH      
  IF @@TRANCOUNT > 0      
        BEGIN      
            ROLLBACK TRANSACTION;      
        END      
        -- Error handling      
        DECLARE @ErrorMessage NVARCHAR(4000);      
        DECLARE @ErrorSeverity INT;      
        DECLARE @ErrorState INT;      
      
        SELECT       
            @ErrorMessage = ERROR_MESSAGE(),      
            @ErrorSeverity = ERROR_SEVERITY(),      
            @ErrorState = ERROR_STATE();      
      
        -- Log the error or return error details      
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);      
      
        -- Set failure result      
        SET @Result = 0;      
    END CATCH      
end   
go