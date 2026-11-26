-- Apply to the school database before deploying the withdrawal API.
-- Configure School__TuitionFeeTypeId with the FeeTypes.Id for tuition in this database.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'school.StudentWithdrawals', N'U') IS NULL
BEGIN
    CREATE TABLE school.StudentWithdrawals (
        Id int IDENTITY PRIMARY KEY,
        StudentEnrollmentId int NOT NULL UNIQUE REFERENCES school.StudentEnrollments(Id),
        WithdrawalDate datetime2 NOT NULL,
        Months decimal(2,1) NOT NULL CHECK (Months BETWEEN 1 AND 6 AND Months * 2 = FLOOR(Months * 2)),
        Balance decimal(18,2) NOT NULL,
        Status int NOT NULL,
        SettlementMethod int NULL,
        InsertDate datetime2 NULL,
        InsertUser nvarchar(450) NULL REFERENCES dbo.AspNetUsers(Id),
        UpdateDate datetime2 NULL,
        UpdateUser nvarchar(450) NULL REFERENCES dbo.AspNetUsers(Id),
        CHECK (
            (Status = 1 AND Balance <> 0 AND SettlementMethod IS NULL AND UpdateDate IS NULL)
            OR (Status = 2 AND Balance <> 0 AND SettlementMethod IS NOT NULL
                AND SettlementMethod IN (1,2,3) AND UpdateDate IS NOT NULL AND UpdateUser IS NOT NULL)
            OR (Status = 3 AND Balance = 0 AND SettlementMethod IS NULL AND UpdateDate IS NULL)
        )
    );
END;

IF OBJECT_ID(N'school.StudentWithdrawalFees', N'U') IS NULL
BEGIN
    CREATE TABLE school.StudentWithdrawalFees (
        Id int IDENTITY PRIMARY KEY,
        StudentWithdrawalId int NOT NULL REFERENCES school.StudentWithdrawals(Id),
        StudentFeeId int NOT NULL REFERENCES school.StudentFees(Id),
        FeeName nvarchar(max) NOT NULL,
        IsTuition bit NOT NULL,
        Original decimal(18,2) NOT NULL,
        Discount decimal(18,2) NOT NULL,
        Net decimal(18,2) NOT NULL,
        Paid decimal(18,2) NOT NULL,
        Due decimal(18,2) NOT NULL,
        UNIQUE (StudentWithdrawalId, StudentFeeId)
    );
END;
COMMIT;
GO

CREATE OR ALTER PROCEDURE [school].[SP_GetAllStudentDataWithFilters]

 @FilterList [dbo].[FilterListTable] Readonly,
 @CurrentPage int,
 @PageSize int,
 @IsFilter bit

AS
BEGIN

	Declare @SearchText nvarchar(Max),@CreatedBy int,@StudentStatus int,@Nationality int,@AcademicStage int,@Gender int,@StudyPeriod int,@StudentType int
	Select @CreatedBy = Count(*) From @FilterList Where CategoryName = 'Users'
	Select @StudentStatus = Count(*) From @FilterList Where CategoryName = 'StudentStatus'
	Select @Nationality = Count(*) From @FilterList Where CategoryName = 'Nationality'
	Select @AcademicStage = Count(*) From @FilterList Where CategoryName = 'AcademicStage'
	Select @Gender = Count(*) From @FilterList Where CategoryName = 'Gender'
	Select @StudyPeriod = Count(*) From @FilterList Where CategoryName = 'StudyPeriod'
	Select @StudentType = Count(*) From @FilterList Where CategoryName = 'StudentType'
	Select @SearchText = ItemId From @FilterList Where CategoryName = 'SearchText'


	Select
	ST.Id,
	SE.Id AS StudentEnrollmentId,
	PA.Id AS ParentId,
	ST.[Code],
	ST.[StudentName],
	PA.Name AS ParentName,
	ASS.Name AS AcademicStageName,
	AY.Name AS AcademicYearName,
	IIF(PA.[ParentPhone] IS NULL OR PA.[ParentPhone] = '',PA.MotherPhone,PA.[ParentPhone]) AS ParentPhone,
	SN.Name AS NationalityName,
	CASE SE.StudentStatusId WHEN 1 THEN N'موجود'  WHEN 2 THEN N'منسحب' WHEN 3 THEN N'محذوف' ELSE N'غير معروف' END AS StudentStatusName,
	ST.[BirthDay],
	PA.[Address],
	IIF(ST.[Gender] = 1,N'ذكر',N'أنثى') AS Gender,
	ST.[Gender] AS GenderId,
	SE.[AcademicStageId],
	SE.StudyPeriodId,
	IIF(SE.StudyPeriodId = 1,N'صباحي',N'مسائي') AS StudyPeriodName,
	ST.[NationalityId],
	SE.[StudentStatusId],
	STS.Id AS StudentTypeId,
	STS.Name AS StudentTypeName,
	IU.UserName as CreatedBy,
	IU.Id as UserId,
	ST.InsertDate,
	FORMAT(ST.InsertDate,'dddd d MMMM , yyyy','ar-AE') as InsertDateStr
	Into #AllData
	From [school].[Students] ST WITH (NOLOCK)
	INNER JOIN [school].[StudentEnrollments] SE WITH (NOLOCK) on ST.Id = SE.StudentId
	INNER Join [school].[Parents] PA WITH (NOLOCK) on ST.ParentId = PA.Id
	INNER Join [school].[AcademicStages] ASS WITH (NOLOCK) on SE.[AcademicStageId] = ASS.Id
	INNER Join [school].[AcademicYears] AY WITH (NOLOCK) on SE.AcademicYearId = AY.Id
	INNER Join [school].[StudentNationalities] SN WITH (NOLOCK) on ST.[NationalityId] = SN.Id
	INNER Join [school].[StudentTypes] STS WITH (NOLOCK) on ST.[StudentTypeId] = STS.Id
	INNER Join [dbo].[AspNetUsers] IU WITH (NOLOCK) on ST.InsertUser = IU.Id

	If(@IsFilter = 1)
		Begin
		    Select N'باسم الطالب, ولي الأمر'as CategoryDisplayName,'SearchText' as CategoryName,'' as ItemId,'' as ItemKey,'' as ItemValue,'SearchText' as FilterType,Cast(1 as bit) as IsVisible
			UNION ALL
			Select N'تاريخ الاضافة'as CategoryDisplayName,'DateRange' as CategoryName,'' as ItemId,'' as ItemKey,'' as ItemValue,'DateRange' as FilterType,Cast(1 as bit) as IsVisible
			UNION ALL
		   Select N'المستخدمين'as CategoryDisplayName,'Users' as CategoryName,Cast(UserId as nvarchar(50)) as ItemId,CreatedBy as ItemKey,Cast(Count(UserId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By UserId,CreatedBy
		   Union All
		   Select N'حالة الطالب'as CategoryDisplayName,'StudentStatus' as CategoryName,Cast(StudentStatusId as nvarchar(50)) as ItemId,CASE StudentStatusId WHEN 1 THEN N'موجود' WHEN 2 THEN N'منسحب' WHEN 3 THEN N'محذوف' ELSE N'غير معروف'
           END as ItemKey,Cast(Count(StudentStatusId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By StudentStatusId
		   Union All
		   Select N'الجنسية'as CategoryDisplayName,'Nationality' as CategoryName,Cast(NationalityId as nvarchar(50)) as ItemId,NationalityName as ItemKey,Cast(Count(NationalityId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By NationalityId,NationalityName
		   --Union All
		   --Select N'نوع الخصم'as CategoryDisplayName,'Categories' as CategoryName,Cast(DiscountTypeId as nvarchar(50)) as ItemId,DiscountTypeName as ItemKey,Cast(Count(DiscountTypeId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   --From #AllData
		   --WHERE DiscountTypeId IS NOT NULL
		   --Group By DiscountTypeId,DiscountTypeName
		   Union All
		   Select N'المرحلة الدراسية'as CategoryDisplayName,'AcademicStage' as CategoryName,Cast(AcademicStageId as nvarchar(50)) as ItemId,AcademicStageName as ItemKey,Cast(Count(AcademicStageId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By AcademicStageId,AcademicStageName
		   Union All
		   Select N'الجنس'as CategoryDisplayName,'Gender' as CategoryName,Cast(GenderId as nvarchar(50)) as ItemId,Gender as ItemKey,Cast(Count(GenderId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By GenderId,Gender
		   Union All
		   Select N'الفترة الدراسية'as CategoryDisplayName,'StudyPeriod' as CategoryName,Cast(StudyPeriodId as nvarchar(50)) as ItemId,StudyPeriodName as ItemKey,Cast(Count(StudyPeriodId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By StudyPeriodId,StudyPeriodName
		   Union All
		   Select N'نوع الطالب'as CategoryDisplayName,'StudentType' as CategoryName,Cast(StudentTypeId as nvarchar(50)) as ItemId,StudentTypeName as ItemKey,Cast(Count(StudentTypeId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By StudentTypeId,StudentTypeName
		End

	Else
		Begin
		   WITH FilteredData AS (
           SELECT
           *
	       FROM #AllData
		   WHERE
			   (@CreatedBy = 0 OR UserId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'Users'))
		   AND (@StudentStatus = 0 OR StudentStatusId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'StudentStatus'))
		   AND (@Nationality = 0 OR NationalityId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'Nationality'))
		   AND (@AcademicStage = 0 OR AcademicStageId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'AcademicStage'))
		   AND (@StudyPeriod = 0 OR StudyPeriodId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'StudyPeriod'))
		   AND (@Gender = 0 OR GenderId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'Gender'))
		   AND (@StudentType = 0 OR StudentTypeId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'StudentType'))
		   AND (@SearchText IS NULL OR dbo.NormalizeArabicText(StudentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%' OR dbo.NormalizeArabicText(ParentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%')
)
		   SELECT
           (SELECT COUNT(*) FROM FilteredData) AS TotalCount,
           FilteredData.*
           FROM FilteredData
		   ORDER BY InsertDate DESC
		   OFFSET ((@CurrentPage - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;


		   DECLARE @DataFields TABLE(DisplayValue  nvarchar(200), DisplayName nvarchar(200),ValueType nvarchar(200))
		   INSERT INTO @DataFields VALUES
		   ('StudentName',N'الاسم','Text'),
		   ('ParentName',N'اسم ولي الاأمر','Text'),
		   ('Phone',N'رقم التلفون','Text'),
		   ('AcademicStageName',N'المرحلة الدراسية','Text'),
		   ('BirthDay',N'تاريخ الميلاد','Text'),
		   ('GovernmentSchool',N'المدرسة الحكومية','Text'),
		   ('NationalityName',N'الجنسية','Text'),
		   ('AcademicYear',N'السنة الدراسية','Text'),
		   ('StudentStatusName',N'الحالة','Text'),
		   ('StudyPeriodName',N'الفترة الدراسية','Text'),
		   ('StudentTypeName',N'نوع الطالب','Text')
		   SELECT * FROM @DataFields
		End

		drop table #AllData
END
GO

CREATE OR ALTER PROCEDURE [school].[SP_GetAllStudentFeeWithFilters]

 @FilterList [dbo].[FilterListTable] Readonly,
 @CurrentPage int,
 @PageSize int,
 @IsFilter bit,
 @FromDate Date = Null,
 @ToDate Date = NULL

AS
BEGIN

	Declare @SearchText nvarchar(Max),@AcademicYear int,@AcademicStage int,@FeedType int,@PaymentStatus int,@DiscountType int
	Select @AcademicYear = Count(*) From @FilterList Where CategoryName = 'AcademicYear'
	Select @AcademicStage = Count(*) From @FilterList Where CategoryName = 'AcademicStage'
	Select @FeedType = Count(*) From @FilterList Where CategoryName = 'FeedType'
	Select @PaymentStatus = Count(*) From @FilterList Where CategoryName = 'PaymentStatus'
	Select @DiscountType = Count(*) From @FilterList Where CategoryName = 'DiscountType'
	Select @SearchText = ItemId From @FilterList Where CategoryName = 'SearchText'


	Select
	SF.Id,
	ST.[Code],
	ST.[StudentName],
	ST.Id AS StudentId,
	ASS.Id AS AcademicStageId,
	AY.Id AS AcademicYearId,
	FT.Id AS FeeTypeId,
	SE.Id AS EnrollmentId,
	FTM.Id AS FeeTemplateId,
	PA.Name AS ParentName,
	ASS.Name AS AcademicStageName,
	AY.Name AS AcademicYearName,
	FT.Name AS FeeTypeName,
	DT.Name AS DiscountType,
	SF.DiscountTypeId,
	SF.DiscountAmountPer,
	SF.DiscountReason,
	SF.TotalAmount,
	SF.DiscountAmount,
	SF.NetAmount,
	SF.PaidAmount,
	SF.RemainingAmount,
	SF.Status AS StatusId,
	LastPayment.NextAmount,
	LastPayment.NextInstallmentDate,
	CASE SF.Status WHEN 1 THEN N'غير مدفوع'  WHEN 2 THEN N'جزئي' WHEN 3 THEN N'مدفوع' WHEN 4 THEN N'ملغي' ELSE N'ملغي' END AS StatusName
	Into #AllData
	From [school].[StudentFees] SF WITH (NOLOCK)
	INNER JOIN [school].[StudentEnrollments] SE on SF.StudentEnrollmentId = SE.Id AND SE.IsCurrent = 1 AND SE.StudentStatusId <> 2
	INNER JOIN [school].[Students] ST WITH (NOLOCK) on SE.StudentId = ST.Id
	INNER Join [school].[Parents] PA WITH (NOLOCK) on ST.ParentId = PA.Id
	INNER Join [school].[AcademicStages] ASS WITH (NOLOCK) on SE.[AcademicStageId] = ASS.Id
	INNER Join [school].[AcademicYears] AY WITH (NOLOCK) on SE.AcademicYearId = AY.Id
	INNER Join [school].[FeeTypes] FT WITH (NOLOCK) on SF.FeeTypeId = FT.Id
	LEFT JOIN [school].[DiscountTypes] DT WITH (NOLOCK) on SF.DiscountTypeId = DT.Id
	LEFT JOIN [school].[FeeTemplates] FTM WITH (NOLOCK) ON FTM.AcademicYearId = SE.AcademicYearId AND FTM.AcademicStageId = SE.AcademicStageId AND FTM.FeeTypeId = SF.FeeTypeId
	OUTER APPLY
	(
		SELECT TOP (1)
		SP.NextInstallmentDate,
		SP.NextAmount
		FROM [school].[StudentPayments] SP WITH (NOLOCK)
		WHERE SP.StudentFeeId = SF.Id
		ORDER BY SP.Id DESC
	) LastPayment;

	If(@IsFilter = 1)
		Begin
		   Select N'باسم الطالب, ولي الأمر, كود الطالب'as CategoryDisplayName,'SearchText' as CategoryName,'' as ItemId,'' as ItemKey,'' as ItemValue,'SearchText' as FilterType,Cast(1 as bit) as IsVisible
		   Union All
		   Select N'اختر تاريخ القسط القادم'as CategoryDisplayName,'DateRange' as CategoryName,'' as ItemId,'' as ItemKey,'' as ItemValue,'DateRange' as FilterType,Cast(1 as bit) as IsVisible
		   UNION ALL
		   Select N'السنة الدراسية'as CategoryDisplayName,'AcademicYear' as CategoryName,Cast(AcademicYearId as nvarchar(50)) as ItemId,AcademicYearName as ItemKey,Cast(Count(AcademicYearId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By AcademicYearId,AcademicYearName
		   Union All
		   Select N'المرحلة الدراسية'as CategoryDisplayName,'AcademicStage' as CategoryName,Cast(AcademicStageId as nvarchar(50)) as ItemId,AcademicStageName as ItemKey,Cast(Count(AcademicStageId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By AcademicStageId,AcademicStageName
		   Union All
		   Select N'نوع الرسوم'as CategoryDisplayName,'FeedType' as CategoryName,Cast(FeeTypeId as nvarchar(50)) as ItemId,FeeTypeName as ItemKey,Cast(Count(FeeTypeId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By FeeTypeId,FeeTypeName
		   Union All
		   Select N'حالة الرسوم'as CategoryDisplayName,'PaymentStatus' as CategoryName,Cast(StatusId as nvarchar(50)) as ItemId,StatusName as ItemKey,Cast(Count(StatusId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By StatusId,StatusName
		   Union All
		   Select N'نوع الخصم'as CategoryDisplayName,'DiscountType' as CategoryName,Cast(DiscountTypeId as nvarchar(50)) as ItemId,DiscountType as ItemKey,Cast(Count(DiscountTypeId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   WHERE DiscountTypeId IS NOT NULL
		   Group By DiscountTypeId,DiscountType
		End

	Else
		Begin
		   WITH FilteredData AS (
           SELECT
           *
	       FROM #AllData
		   WHERE  (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(DATE, NextInstallmentDate) BETWEEN @FromDate AND @ToDate)
		   AND (@AcademicYear = 0 OR AcademicYearId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'AcademicYear'))
		   AND (@AcademicStage = 0 OR AcademicStageId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'AcademicStage'))
		   AND (@FeedType = 0 OR FeeTypeId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'FeedType'))
		   AND (@PaymentStatus = 0 OR StatusId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'PaymentStatus'))
		   AND (@DiscountType = 0 OR DiscountTypeId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'DiscountType'))
		   AND (@SearchText IS NULL OR dbo.NormalizeArabicText(StudentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%' OR dbo.NormalizeArabicText(ParentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%' OR Code LIKE '%' + @SearchText + '%')
		   )
		   SELECT
           (SELECT COUNT(*) FROM FilteredData) AS TotalCount,
           FilteredData.*
           FROM FilteredData
		   ORDER BY Code DESC
		   OFFSET ((@CurrentPage - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;


		   DECLARE @DataFields TABLE(DisplayValue  nvarchar(200), DisplayName nvarchar(200),ValueType nvarchar(200))
		   INSERT INTO @DataFields VALUES
		   ('Code',N'كود الطالب','Text'),
		   ('StudentName',N'اسم الطالب','Text'),
		   ('ParentName',N'اسم ولي الأمر','Text'),
		   ('AcademicStageName',N'المرحلة الدراسية','Text'),
		   ('AcademicYearName',N'السنة الدراسية','Text'),
		   ('FeeTypeName',N'نوع الرسوم','Text'),
		   ('TotalAmount',N'إجمالي الرسوم','Number'),
		   ('DiscountAmountPer',N'نسبة الخصم','Text'),
		   ('DiscountAmount',N'الخصم','Number'),
		   ('NetAmount',N'الصافي','Number'),
		   ('PaidAmount',N'المدفوع','Number'),
		   ('RemainingAmount',N'المتبقي','Number'),
		   ('StatusName',N'الحالة','Text')
		   SELECT * FROM @DataFields
		End

		drop table #AllData
END
GO

CREATE OR ALTER PROCEDURE [school].[SP_GetAllStudentPaymentWithFilters]

    @FilterList [dbo].[FilterListTable] READONLY,
    @CurrentPage INT,
    @PageSize INT,
    @IsFilter BIT,
    @FromDate Date = Null,
    @ToDate Date = NULL

AS
BEGIN

    DECLARE @SearchText NVARCHAR(MAX), @AcademicYear INT,@AcademicStage INT,@FeedType INT, @PaymentMethod INT,@PaymentStatus INT

    SELECT @AcademicYear = COUNT(*) FROM @FilterList WHERE CategoryName = 'AcademicYear'
    SELECT @AcademicStage = COUNT(*) FROM @FilterList WHERE CategoryName = 'AcademicStage'
    SELECT @FeedType = COUNT(*) FROM @FilterList WHERE CategoryName = 'FeedType'
    SELECT @PaymentMethod = COUNT(*) FROM @FilterList WHERE CategoryName = 'PaymentMethod'
    SELECT @PaymentStatus = COUNT(*) FROM @FilterList WHERE CategoryName = 'PaymentStatus'
    SELECT @SearchText = ItemId FROM @FilterList WHERE CategoryName = 'SearchText'

    SELECT
    SP.Id,
    SP.StudentFeeId,
    SP.ReceiptNumber,
    SP.Amount,
    SP.PaymentMethod,
    CASE SP.PaymentMethod  WHEN 1 THEN N'كاش'  WHEN 2 THEN N'انستاباي'  WHEN 3 THEN N'فودافون كاش' ELSE N'-' END AS PaymentMethodName,
    SP.PaymentDate,
    SP.NextAmount,
    SP.NextInstallmentDate,
    SP.Note,
    SP.InsertUser,
    SP.InsertDate,
    SP.IsCancelled,
    SP.CancelledBy,
    SP.CancelledDate,
    CASE WHEN SP.IsCancelled = 1 THEN N'ملغي' ELSE N'مدفوع' END AS PaymentStatusName,
    ST.Id AS StudentId,
    ST.Code,
    ST.StudentName,
    PA.Name AS ParentName,
    SE.Id AS EnrollmentId,
    ASS.Id AS AcademicStageId,
    ASS.Name AS AcademicStageName,
    AY.Id AS AcademicYearId,
    AY.Name AS AcademicYearName,
    FT.Id AS FeeTypeId,
    FT.Name AS FeeTypeName,
    SF.TotalAmount,
    SF.DiscountAmount,
    SF.DiscountAmountPer,
    SF.NetAmount,
    SF.PaidAmount,
    SF.RemainingAmount,
    SF.Status,
	US.UserName AS InsertUserName,
	USS.UserName AS CancelledByName
    INTO #AllData
    FROM school.StudentPayments SP WITH (NOLOCK)
    INNER JOIN school.StudentFees SF WITH (NOLOCK) ON SP.StudentFeeId = SF.Id
    INNER JOIN school.StudentEnrollments SE ON SF.StudentEnrollmentId = SE.Id AND SE.IsCurrent = 1 AND SE.StudentStatusId <> 2
    INNER JOIN school.Students ST WITH (NOLOCK) ON ST.Id = SE.StudentId
    INNER JOIN school.Parents PA WITH (NOLOCK) ON PA.Id = ST.ParentId
    INNER JOIN school.AcademicStages ASS WITH (NOLOCK) ON ASS.Id = SE.AcademicStageId
    INNER JOIN school.AcademicYears AY WITH (NOLOCK) ON AY.Id = SE.AcademicYearId
    INNER JOIN school.FeeTypes FT WITH (NOLOCK) ON FT.Id = SF.FeeTypeId
    INNER JOIN [dbo].[AspNetUsers] US WITH (NOLOCK) ON SP.InsertUser = US.Id
    LEFT JOIN [dbo].[AspNetUsers] USS WITH (NOLOCK) ON SP.CancelledBy = USS.Id

    IF (@IsFilter = 1)
    BEGIN
        SELECT N'باسم الطالب، ولي الأمر، كود الطالب، رقم الإيصال' AS CategoryDisplayName,'SearchText' AS CategoryName,'' AS ItemId,'' AS ItemKey,'' AS ItemValue,'SearchText' AS FilterType,CAST(1 AS BIT) AS IsVisible
        UNION ALL
        Select N'اختر تاريخ القسط القادم'as CategoryDisplayName,'DateRange' as CategoryName,'' as ItemId,'' as ItemKey,'' as ItemValue,'DateRange' as FilterType,Cast(1 as bit) as IsVisible
		UNION ALL
        SELECT N'السنة الدراسية','AcademicYear',CAST(AcademicYearId AS NVARCHAR(50)),AcademicYearName,CAST(COUNT(*) AS NVARCHAR(50)),'Checkbox',CAST(1 AS BIT)
        FROM #AllData
        GROUP BY AcademicYearId,AcademicYearName
        UNION ALL
        SELECT N'المرحلة الدراسية','AcademicStage',CAST(AcademicStageId AS NVARCHAR(50)),AcademicStageName,CAST(COUNT(*) AS NVARCHAR(50)),'Checkbox',CAST(1 AS BIT)
        FROM #AllData
        GROUP BY AcademicStageId,AcademicStageName
        UNION ALL
        SELECT N'نوع الرسوم','FeedType',CAST(FeeTypeId AS NVARCHAR(50)),FeeTypeName,CAST(COUNT(*) AS NVARCHAR(50)),'Checkbox',CAST(1 AS BIT)
        FROM #AllData
        GROUP BY FeeTypeId,FeeTypeName
        UNION ALL
        SELECT N'طريقة الدفع','PaymentMethod',CAST(PaymentMethod AS NVARCHAR(50)),PaymentMethodName,CAST(COUNT(*) AS NVARCHAR(50)),'Checkbox',CAST(1 AS BIT)
        FROM #AllData
        GROUP BY PaymentMethod,PaymentMethodName
        UNION ALL
        SELECT N'حالة الدفعة','PaymentStatus',CAST(CAST(IsCancelled AS INT) AS NVARCHAR(50)),PaymentStatusName,CAST(COUNT(*) AS NVARCHAR(50)),'Checkbox',CAST(1 AS BIT)
        FROM #AllData
        GROUP BY IsCancelled,PaymentStatusName
    END
	ELSE
    BEGIN
		;WITH FilteredData AS
		(
			SELECT *
			FROM #AllData
			WHERE (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(DATE, NextInstallmentDate) BETWEEN @FromDate AND @ToDate)
			AND	(@AcademicYear = 0 OR AcademicYearId IN (SELECT ItemId FROM @FilterList WHERE CategoryName='AcademicYear'))
			AND (@AcademicStage = 0 OR AcademicStageId IN (SELECT ItemId FROM @FilterList WHERE CategoryName='AcademicStage'))
			AND (@FeedType = 0 OR FeeTypeId IN (SELECT ItemId FROM @FilterList WHERE CategoryName='FeedType'))
			AND (@PaymentMethod = 0 OR PaymentMethod IN (SELECT ItemId FROM @FilterList WHERE CategoryName='PaymentMethod'))
			AND (@PaymentStatus = 0 OR CAST(IsCancelled AS INT) IN (SELECT ItemId FROM @FilterList WHERE CategoryName='PaymentStatus'))
			AND (@SearchText IS NULL OR dbo.NormalizeArabicText(StudentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%' OR dbo.NormalizeArabicText(ParentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%'
				OR Code LIKE '%' + @SearchText + '%' OR ReceiptNumber LIKE '%' + @SearchText + '%')
		)

		SELECT
		(SELECT COUNT(*) FROM FilteredData) AS TotalCount,
		FilteredData.*
		FROM FilteredData
		ORDER BY PaymentDate DESC,Id DESC
		OFFSET ((@CurrentPage-1)*@PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY;

		DECLARE @DataFields TABLE(DisplayValue NVARCHAR(200),DisplayName NVARCHAR(200),ValueType NVARCHAR(200))
		INSERT INTO @DataFields
		VALUES
		('ReceiptNumber',N'رقم الإيصال','Text'),
		('Code',N'كود الطالب','Text'),
		('StudentName',N'اسم الطالب','Text'),
		('ParentName',N'اسم ولي الأمر','Text'),
		('AcademicStageName',N'المرحلة الدراسية','Text'),
		('AcademicYearName',N'السنة الدراسية','Text'),
		('FeeTypeName',N'نوع الرسوم','Text'),
		('Amount',N'قيمة الدفعة','Number'),
		('PaymentMethodName',N'طريقة الدفع','Text'),
		('PaymentDate',N'تاريخ الدفع','Date'),
		('NextAmount',N'القسط القادم','Number'),
		('NextInstallmentDate',N'موعد القسط القادم','Date'),
		('RemainingAmount',N'المتبقي','Number'),
		('PaymentStatusName',N'الحالة','Text'),
		('InsertUser',N'الموظف','Text'),
		('InsertDate',N'تاريخ الإنشاء','Date'),
		('CancelledBy',N'ألغيت بواسطة','Text'),
		('CancelledDate',N'تاريخ الإلغاء','Date'),
		('Note',N'ملاحظات','Text');

		SELECT * FROM @DataFields;
	END

DROP TABLE #AllData;

END
GO

CREATE OR ALTER PROCEDURE [school].[SP_GetAllStudentHasNextInstallment]

 @FilterList [dbo].[FilterListTable] Readonly,
 @FromDate Date = Null,
 @ToDate Date = NULL,
 @CurrentPage int,
 @PageSize int,
 @IsFilter bit

AS
BEGIN

	Declare @SearchText nvarchar(Max),@AcademicYear int,@AcademicStage int,@FeedType int,@PaymentStatus int,@DiscountType int
	Select @AcademicYear = Count(*) From @FilterList Where CategoryName = 'AcademicYear'
	Select @AcademicStage = Count(*) From @FilterList Where CategoryName = 'AcademicStage'
	Select @FeedType = Count(*) From @FilterList Where CategoryName = 'FeedType'
	Select @PaymentStatus = Count(*) From @FilterList Where CategoryName = 'PaymentStatus'
	Select @DiscountType = Count(*) From @FilterList Where CategoryName = 'DiscountType'
	Select @SearchText = ItemId From @FilterList Where CategoryName = 'SearchText'


	Select
	SF.Id,
	ST.[Code],
	ST.[StudentName],
	ST.Id AS StudentId,
	ASS.Id AS AcademicStageId,
	AY.Id AS AcademicYearId,
	FT.Id AS FeeTypeId,
	SE.Id AS EnrollmentId,
	FTM.Id AS FeeTemplateId,
	PA.Name AS ParentName,
	ASS.Name AS AcademicStageName,
	AY.Name AS AcademicYearName,
	FT.Name AS FeeTypeName,
	DT.Name AS DiscountType,
	SF.DiscountTypeId,
	SF.DiscountAmountPer,
	SF.DiscountReason,
	SF.TotalAmount,
	SF.DiscountAmount,
	SF.NetAmount,
	SF.PaidAmount,
	SF.RemainingAmount,
	SF.Status AS StatusId,
	LastPayment.NextAmount,
	LastPayment.NextInstallmentDate,
	CASE SF.Status WHEN 1 THEN N'غير مدفوع'  WHEN 2 THEN N'جزئي' WHEN 3 THEN N'مدفوع' ELSE N'ملغي' END AS StatusName
	Into #AllData
	From [school].[StudentFees] SF WITH (NOLOCK)
	INNER JOIN [school].[StudentEnrollments] SE on SF.StudentEnrollmentId = SE.Id AND SE.IsCurrent = 1 AND SE.StudentStatusId <> 2
	INNER JOIN [school].[Students] ST WITH (NOLOCK) on SE.StudentId = ST.Id
	INNER Join [school].[Parents] PA WITH (NOLOCK) on ST.ParentId = PA.Id
	INNER Join [school].[AcademicStages] ASS WITH (NOLOCK) on SE.[AcademicStageId] = ASS.Id
	INNER Join [school].[AcademicYears] AY WITH (NOLOCK) on SE.AcademicYearId = AY.Id
	INNER Join [school].[FeeTypes] FT WITH (NOLOCK) on SF.FeeTypeId = FT.Id
	LEFT JOIN [school].[DiscountTypes] DT WITH (NOLOCK) on SF.DiscountTypeId = DT.Id
	LEFT JOIN [school].[FeeTemplates] FTM WITH (NOLOCK) ON FTM.AcademicYearId = SE.AcademicYearId AND FTM.AcademicStageId = SE.AcademicStageId AND FTM.FeeTypeId = SF.FeeTypeId
	OUTER APPLY
	(
		SELECT TOP (1)
		SP.NextInstallmentDate,
		SP.NextAmount
		FROM [school].[StudentPayments] SP WITH (NOLOCK)
		WHERE SP.StudentFeeId = SF.Id
		ORDER BY SP.Id DESC
	) LastPayment
	WHERE LastPayment.NextAmount IS NOT NULL

	If(@IsFilter = 1)
		Begin
		   Select N'باسم الطالب, ولي الأمر, كود الطالب'as CategoryDisplayName,'SearchText' as CategoryName,'' as ItemId,'' as ItemKey,'' as ItemValue,'SearchText' as FilterType,Cast(1 as bit) as IsVisible
		   Union All
		   Select N'تاريخ القسط القادم'as CategoryDisplayName,'DateRange' as CategoryName,'' as ItemId,'' as ItemKey,'' as ItemValue,'DateRange' as FilterType,Cast(1 as bit) as IsVisible
		   Union All
		   Select N'السنة الدراسية'as CategoryDisplayName,'AcademicYear' as CategoryName,Cast(AcademicYearId as nvarchar(50)) as ItemId,AcademicYearName as ItemKey,Cast(Count(AcademicYearId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By AcademicYearId,AcademicYearName
		   Union All
		   Select N'المرحلة الدراسية'as CategoryDisplayName,'AcademicStage' as CategoryName,Cast(AcademicStageId as nvarchar(50)) as ItemId,AcademicStageName as ItemKey,Cast(Count(AcademicStageId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By AcademicStageId,AcademicStageName
		   Union All
		   Select N'نوع الرسوم'as CategoryDisplayName,'FeedType' as CategoryName,Cast(FeeTypeId as nvarchar(50)) as ItemId,FeeTypeName as ItemKey,Cast(Count(FeeTypeId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By FeeTypeId,FeeTypeName
		   Union All
		   Select N'حالة الرسوم'as CategoryDisplayName,'PaymentStatus' as CategoryName,Cast(StatusId as nvarchar(50)) as ItemId,StatusName as ItemKey,Cast(Count(StatusId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   Group By StatusId,StatusName
		   Union All
		   Select N'نوع الخصم'as CategoryDisplayName,'DiscountType' as CategoryName,Cast(DiscountTypeId as nvarchar(50)) as ItemId,DiscountType as ItemKey,Cast(Count(DiscountTypeId) as nvarchar(50)) as ItemValue,'Checkbox' as FilterType,Cast(1 as bit) as IsVisible
		   From #AllData
		   WHERE DiscountTypeId IS NOT NULL
		   Group By DiscountTypeId,DiscountType
		End

	Else
		Begin
		   WITH FilteredData AS (
           SELECT
           *
	       FROM #AllData
		   WHERE (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(DATE, NextInstallmentDate) BETWEEN @FromDate AND @ToDate)
		   AND (@AcademicYear = 0 OR AcademicYearId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'AcademicYear'))
		   AND (@AcademicStage = 0 OR AcademicStageId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'AcademicStage'))
		   AND (@FeedType = 0 OR FeeTypeId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'FeedType'))
		   AND (@PaymentStatus = 0 OR StatusId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'PaymentStatus'))
		   AND (@DiscountType = 0 OR DiscountTypeId IN (SELECT ItemId FROM @FilterList WHERE CategoryName = 'DiscountType'))
		   AND (@SearchText IS NULL OR dbo.NormalizeArabicText(StudentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%' OR dbo.NormalizeArabicText(ParentName) LIKE '%' + dbo.NormalizeArabicText(@SearchText) + '%' OR Code LIKE '%' + @SearchText + '%')
		   )
		   SELECT
           (SELECT COUNT(*) FROM FilteredData) AS TotalCount,
           FilteredData.*
           FROM FilteredData
		   ORDER BY Code DESC
		   OFFSET ((@CurrentPage - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;


		   DECLARE @DataFields TABLE(DisplayValue  nvarchar(200), DisplayName nvarchar(200),ValueType nvarchar(200))
		   INSERT INTO @DataFields VALUES
		   ('Code',N'كود الطالب','Text'),
		   ('StudentName',N'اسم الطالب','Text'),
		   ('ParentName',N'اسم ولي الأمر','Text'),
		   ('AcademicStageName',N'المرحلة الدراسية','Text'),
		   ('AcademicYearName',N'السنة الدراسية','Text'),
		   ('FeeTypeName',N'نوع الرسوم','Text'),
		   ('TotalAmount',N'إجمالي الرسوم','Number'),
		   ('DiscountAmountPer',N'نسبة الخصم','Text'),
		   ('DiscountAmount',N'الخصم','Number'),
		   ('NetAmount',N'الصافي','Number'),
		   ('PaidAmount',N'المدفوع','Number'),
		   ('RemainingAmount',N'المتبقي','Number'),
		   ('StatusName',N'الحالة','Text')
		   SELECT * FROM @DataFields
		End

		drop table #AllData
END
GO

CREATE OR ALTER PROCEDURE [school].[SP_GetCurrentAcademicYearFinancialSummary]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentAcademicYearId INT;

    SELECT TOP (1)
    @CurrentAcademicYearId = Id
    FROM [school].[AcademicYears]
    WHERE IsCurrent = 1;


    IF @CurrentAcademicYearId IS NULL
    BEGIN
        SELECT
            CAST(0 AS FLOAT) AS TotalCanceledFees,
            CAST(0 AS FLOAT) AS TotalFees,
            CAST(0 AS FLOAT) AS TotalCashPaid,
            CAST(0 AS FLOAT) AS TotalVodafoneCashPaid,
            CAST(0 AS FLOAT) AS TotalInstaPayPaid,
            CAST(0 AS FLOAT) AS TotalDiscounts,
            CAST(0 AS FLOAT) AS TotalPaid,
            CAST(0 AS FLOAT) AS TotalRemaining;

        RETURN;
    END;

    ;WITH FeesSummary AS
    (
        SELECT
        SUM(ISNULL(SF.TotalAmount, 0)) AS TotalFees,
        SUM(ISNULL(SF.DiscountAmount, 0)) AS TotalDiscounts,
        SUM(ISNULL(SF.NetAmount, 0)) AS TotalNetFees
        FROM [school].[StudentFees] SF
        INNER JOIN [school].[StudentEnrollments] SE ON SE.Id = SF.StudentEnrollmentId
        WHERE SE.StudentStatusId <> 2 AND SE.AcademicYearId = @CurrentAcademicYearId AND SF.Status <> 4
    ),

	CanceledFeesSummary AS
    (
        SELECT
        SUM(ISNULL(SF.TotalAmount, 0)) AS TotalCanceledFees
        FROM [school].[StudentFees] SF
        INNER JOIN [school].[StudentEnrollments] SE ON SE.Id = SF.StudentEnrollmentId
        WHERE SE.StudentStatusId <> 2 AND SE.AcademicYearId = @CurrentAcademicYearId AND SF.Status = 4
    ),

    PaymentsSummary AS
    (
        SELECT
        SUM(ISNULL(SP.Amount, 0)) AS TotalPaid,
        SUM(CASE WHEN SP.PaymentMethod = 1 THEN ISNULL(SP.Amount, 0) ELSE 0 END) AS TotalCashPaid,
		SUM(CASE WHEN SP.PaymentMethod = 3 THEN ISNULL(SP.Amount, 0) ELSE 0 END) AS TotalVodafoneCashPaid,
        SUM(CASE WHEN SP.PaymentMethod = 2 THEN ISNULL(SP.Amount, 0) ELSE 0 END) AS TotalInstaPayPaid

        FROM [school].[StudentPayments] SP
        INNER JOIN [school].[StudentFees] SF ON SF.Id = SP.StudentFeeId AND SF.Status <> 4
        INNER JOIN [school].[StudentEnrollments] SE ON SE.Id = SF.StudentEnrollmentId
        WHERE SE.StudentStatusId <> 2 AND SE.AcademicYearId = @CurrentAcademicYearId AND SP.IsCancelled = 0
    )

    SELECT
	ISNULL(CFS.TotalCanceledFees, 0) AS TotalCanceledFees,
    ISNULL(FS.TotalFees, 0) AS TotalFees,
    ISNULL(PS.TotalCashPaid, 0) AS TotalCashPaid,
    ISNULL(PS.TotalVodafoneCashPaid, 0) AS TotalVodafoneCashPaid,
    ISNULL(PS.TotalInstaPayPaid, 0) AS TotalInstaPayPaid,
    ISNULL(FS.TotalDiscounts, 0) AS TotalDiscounts,
    ISNULL(PS.TotalPaid, 0) AS TotalPaid,
    ISNULL(FS.TotalNetFees, 0) - ISNULL(PS.TotalPaid, 0) AS TotalRemaining
    FROM FeesSummary FS
	CROSS JOIN CanceledFeesSummary CFS
    CROSS JOIN PaymentsSummary PS;

END
GO
