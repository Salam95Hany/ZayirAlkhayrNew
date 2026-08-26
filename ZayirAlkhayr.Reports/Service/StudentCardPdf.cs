using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Reports.Service
{
    public sealed class StudentCardPdf : IDocument
    {
        private readonly IReadOnlyList<StudentCardData> _students;
        private readonly byte[] _backgroundImage;
        private const string CairoFont = "Cairo";
        private const float PageMarginHorizontalMm = 0.5f;
        private const float PageMarginVerticalMm = 0.5f;
        private const float CardHorizontalSpacingMm = 0.2f;
        private const float CardVerticalSpacingMm = 0.2f;
        private const float CardWidthMm = 104.3f;
        private const float CardHeightMm = 73.8f;
        private const float CardOuterBorderPoints = 0.55f;
        private const string CardBorderColor = "#15585A";
        private const float ContentRightMm = 3.7f;
        private const float ContentLeftMm = 5.0f;
        private const float InformationTopMm = 26.5f;
        private const float NameRowHeightMm = 8.2f;
        private const float GradeRowHeightMm = 8.2f;
        private const float PaymentRowHeightMm = 9.0f;
        private const float InformationRowGapMm = 0.5f;
        private const float SlotBadgeSizeMm = 7.0f;
        private const float SlotTopMm = 1.8f;
        private const float SlotLeftMm = 1.8f;
        private const float SlotCornerRadiusMm = 1.4f;
        private const string SlotBackgroundColor = "#075B59";
        private const string SlotTextColor = "#FFFFFF";
        private const float SlotFontSize = 8.5f;
        private const float NameFontSize = 12.0f;
        private const float GradeFontSize = 11.5f;
        private const float PaymentFontSize = 10.5f;
        private const string MainTextColor = "#111827";
        private const string InstallmentColor = "#B86A00";
        private const string PaidColor = "#007C68";

        public StudentCardPdf(IReadOnlyList<StudentCardData> students, byte[] backgroundImage)
        {
            ArgumentNullException.ThrowIfNull(students);
            ArgumentNullException.ThrowIfNull(backgroundImage);

            if (backgroundImage.Length == 0)
                throw new ArgumentException("Background image cannot be empty.", nameof(backgroundImage));

            _students = students;
            _backgroundImage = backgroundImage;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = "Student Cards",
                Author = "Bashaer Quran",
                Subject = "Student payment cards",
                Creator = "QuestPDF 2025.7.2"
            };
        }

        public void Compose(IDocumentContainer container)
        {
            var pages = BuildPages();

            if (pages.Count == 0)
            {
                ComposePage(container, Array.Empty<StudentCardData>());
                return;
            }

            foreach (var pageStudents in pages)
                ComposePage(container, pageStudents);
        }

        private void ComposePage(IDocumentContainer document, IReadOnlyList<StudentCardData> students)
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Portrait());
                page.MarginHorizontal(PageMarginHorizontalMm, Unit.Millimetre);
                page.MarginVertical(PageMarginVerticalMm, Unit.Millimetre);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(style => style.FontFamily(CairoFont).FontSize(NameFontSize).FontColor(MainTextColor));
                page.Content().Element(container => ComposePageGrid(container, students));
            });
        }

        private void ComposePageGrid(IContainer container, IReadOnlyList<StudentCardData> students)
        {
            container.Column(column =>
            {
                column.Spacing(CardVerticalSpacingMm, Unit.Millimetre);

                for (var rowIndex = 0; rowIndex < 4; rowIndex++)
                {
                    var rightSlot = rowIndex * 2 + 1;
                    var leftSlot = rightSlot + 1;
                    var rightStudent = FindStudentBySlot(students, rightSlot);
                    var leftStudent = FindStudentBySlot(students, leftSlot);

                    column.Item().Height(CardHeightMm, Unit.Millimetre).Row(row =>
                    {
                        row.ConstantItem(CardWidthMm, Unit.Millimetre).Element(card => ComposeCardOrEmpty(card, rightStudent));
                        row.ConstantItem(CardHorizontalSpacingMm, Unit.Millimetre);
                        row.ConstantItem(CardWidthMm, Unit.Millimetre).Element(card => ComposeCardOrEmpty(card, leftStudent));
                    });
                }
            });
        }

        private static StudentCardData? FindStudentBySlot(IReadOnlyList<StudentCardData> students, int slotNumber)
        {
            return students.FirstOrDefault(x => x.SlotNumber == slotNumber);
        }

        private void ComposeCardOrEmpty(IContainer container, StudentCardData? student)
        {
            if (student is null)
                return;

            ComposeCard(container, student);
        }

        private void ComposeCard(IContainer container, StudentCardData student)
        {
            container.Height(CardHeightMm, Unit.Millimetre).Border(CardOuterBorderPoints).BorderColor(CardBorderColor)
                .CornerRadius(1.1f, Unit.Millimetre)
                .Layers(layers =>
                {
                    layers.Layer().Image(_backgroundImage).FitArea();
                    layers.PrimaryLayer().ContentFromRightToLeft().Layers(cardLayers =>
                    {
                        cardLayers.PrimaryLayer().Element(content => ComposeStudentInformation(content, student));
                        //cardLayers.Layer().Element(slot => ComposeSlotBadge(slot, student.SlotNumber));
                    });
                });
        }

        private void ComposeStudentInformation(IContainer container, StudentCardData student)
        {
            container.PaddingTop(InformationTopMm, Unit.Millimetre).PaddingRight(ContentRightMm, Unit.Millimetre)
                .PaddingLeft(ContentLeftMm, Unit.Millimetre).Column(column =>
                {
                    column.Spacing(InformationRowGapMm, Unit.Millimetre);
                    column.Item().Height(NameRowHeightMm, Unit.Millimetre).Element(c => ComposeName(c, student));
                    column.Item().Height(GradeRowHeightMm, Unit.Millimetre).Element(c => ComposeGrade(c, student));
                    column.Item().Height(PaymentRowHeightMm, Unit.Millimetre).Element(c => ComposePaymentStatus(c, student));
                });
        }

        private void ComposeName(IContainer container, StudentCardData student)
        {
            container.ContentFromRightToLeft().AlignRight().AlignMiddle().ScaleToFit().Text(text =>
                {
                    text.AlignRight();
                    text.DefaultTextStyle(style => style.FontFamily(CairoFont).FontSize(NameFontSize).FontColor(MainTextColor).SemiBold());
                    text.Span("الاسم: ");
                    text.Span(student.StudentName ?? string.Empty);
                });
        }

        private void ComposeGrade(IContainer container, StudentCardData student)
        {
            container.ContentFromRightToLeft().AlignRight().AlignMiddle().ScaleToFit().Text(text =>
            {
                text.AlignRight();
                text.DefaultTextStyle(style => style.FontFamily(CairoFont).FontSize(GradeFontSize).FontColor(MainTextColor).SemiBold());
                text.Span("الصف: ");
                text.Span(student.GradeName ?? string.Empty);
            });
        }

        private void ComposePaymentStatus(IContainer container, StudentCardData student)
        {
            if (student.InstallmentRenewalDate.HasValue)
            {
                ComposeInstallmentRenewal(container, student.InstallmentRenewalDate.Value);
                return;
            }

            ComposePaidInFull(container);
        }

        private void ComposeInstallmentRenewal(IContainer container, DateTime renewalDate)
        {
            container.ContentFromRightToLeft().AlignRight().AlignMiddle().Row(row =>
            {
                row.AutoItem().AlignMiddle().Element(CalendarIcon);
                row.ConstantItem(1.3f, Unit.Millimetre);
                row.RelativeItem().AlignMiddle().ScaleToFit().Text(text =>
                {
                    text.AlignRight();
                    text.DefaultTextStyle(style => style.FontFamily(CairoFont).FontSize(PaymentFontSize).FontColor(InstallmentColor).SemiBold());
                    text.Span("موعد تجديد القسط: ");
                    text.Span(renewalDate.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture)).FontColor(InstallmentColor);
                });
            });
        }

        private void ComposePaidInFull(IContainer container)
        {
            container.ContentFromRightToLeft().AlignRight().AlignMiddle().Row(row =>
            {
                row.AutoItem().AlignMiddle().Element(CheckCircleIcon);
                row.ConstantItem(1.3f, Unit.Millimetre);
                row.RelativeItem().AlignMiddle().ScaleToFit().Text(text =>
                {
                    text.AlignRight();
                    text.Span("تم السداد بالكامل").FontFamily(CairoFont).FontSize(PaymentFontSize + 1).FontColor(PaidColor).SemiBold();
                });
            });
        }

        private void ComposeSlotBadge(IContainer container, int slotNumber)
        {
            container.ContentFromLeftToRight().PaddingTop(SlotTopMm, Unit.Millimetre).PaddingLeft(SlotLeftMm, Unit.Millimetre).AlignLeft().AlignTop()
            .Width(SlotBadgeSizeMm, Unit.Millimetre).Height(SlotBadgeSizeMm, Unit.Millimetre).Background(SlotBackgroundColor)
            .CornerRadius(SlotCornerRadiusMm, Unit.Millimetre).AlignCenter().AlignMiddle().Text(slotNumber.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .FontFamily(CairoFont).FontSize(SlotFontSize).FontColor(SlotTextColor).SemiBold();
        }

        private static void CalendarIcon(IContainer container)
        {
            container.Width(4.4f, Unit.Millimetre).Height(4.4f, Unit.Millimetre).Svg(
                    """
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 24 24">

                  <g
                    fill="none"
                    stroke="#B86A00"
                    stroke-width="1.8"
                    stroke-linecap="round"
                    stroke-linejoin="round">

                    <rect
                        x="3"
                        y="5"
                        width="18"
                        height="16"
                        rx="2" />

                    <path d="M16 3v4" />
                    <path d="M8 3v4" />
                    <path d="M3 10h18" />

                    <path d="M8 14h.01" />
                    <path d="M12 14h.01" />
                    <path d="M16 14h.01" />

                    <path d="M8 17h.01" />
                    <path d="M12 17h.01" />
                    <path d="M16 17h.01" />
                  </g>
                </svg>
                """).FitArea();
        }

        private static void CheckCircleIcon(IContainer container)
        {
            container.Width(5.0f, Unit.Millimetre).Height(5.0f, Unit.Millimetre).Svg(
                    """
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 24 24">

                  <g
                    fill="none"
                    stroke="#007C68"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round">

                    <circle
                        cx="12"
                        cy="12"
                        r="9" />

                    <path
                        d="M8 12.5l2.5 2.5L16 9.5" />
                  </g>
                </svg>
                """).FitArea();
        }

        private IReadOnlyList<IReadOnlyList<StudentCardData>> BuildPages()
        {
            if (_students.Count == 0)
                return Array.Empty<IReadOnlyList<StudentCardData>>();

            var result =
                new List<IReadOnlyList<StudentCardData>>();

            for (var pageStart = 0; pageStart < _students.Count; pageStart += 8)
            {
                var pageSource = _students.Skip(pageStart).Take(8).ToList();
                var page = NormalizePageSlots(pageSource);
                result.Add(page);
            }

            return result;
        }

        private static IReadOnlyList<StudentCardData> NormalizePageSlots(IReadOnlyList<StudentCardData> source)
        {
            var result = new List<StudentCardData>(source.Count);
            var occupiedSlots = new HashSet<int>();

            for (var index = 0; index < source.Count; index++)
            {
                var original = source[index];
                var slot = original.SlotNumber;

                if (slot == 0)
                    slot = index + 1;

                if (slot is < 1 or > 8)
                {
                    throw new InvalidOperationException($"StudentId {original.StudentId} has invalid " + $"SlotNumber {slot}. " + "SlotNumber must be between 1 and 8.");
                }

                if (!occupiedSlots.Add(slot))
                {
                    throw new InvalidOperationException($"Duplicate SlotNumber {slot} was found " + "inside the same page.");
                }

                result.Add(new StudentCardData
                {
                    StudentId = original.StudentId,
                    SlotNumber = slot,
                    StudentName = original.StudentName,
                    GradeName = original.GradeName,
                    InstallmentRenewalDate = original.InstallmentRenewalDate
                });
            }

            return result;
        }
    }
}
