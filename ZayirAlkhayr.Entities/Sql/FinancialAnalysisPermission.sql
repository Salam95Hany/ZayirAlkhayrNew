-- Register the page in the existing permissions tree. Assign accountants from the user permissions screen.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @ParentId nvarchar(128), @PageId nvarchar(128);
SELECT @ParentId = r.Id FROM dbo.AspNetRoles r
JOIN config.Applications a ON a.ApplicationId = r.Id
WHERE r.Name = N'School_ManageAccounts';
IF @ParentId IS NULL THROW 50001, N'لم يتم العثور على قسم إدارة حسابات المدرسة.', 1;
SELECT @PageId = Id FROM dbo.AspNetRoles WITH (UPDLOCK, HOLDLOCK)
WHERE NormalizedName = N'SCHOOL_MANAGEACCOUNTS_FINANCIALANALYSIS';
IF @PageId IS NULL
BEGIN
    SET @PageId = CONVERT(nvarchar(36), NEWID());
    INSERT dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@PageId, N'School_ManageAccounts_FinancialAnalysis', N'SCHOOL_MANAGEACCOUNTS_FINANCIALANALYSIS', NEWID());
END;
IF NOT EXISTS (SELECT 1 FROM config.Applications WITH (UPDLOCK, HOLDLOCK) WHERE ApplicationId = @PageId)
    INSERT config.Applications (ApplicationId, ParentId, ApplicationName, IsActive, DisplayOrder)
    SELECT @PageId, @ParentId, N'التحليل المالي', 1, COALESCE(MAX(DisplayOrder), 0) + 1
    FROM config.Applications WHERE ParentId = @ParentId;
COMMIT;
