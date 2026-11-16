using Droosy_DAL.Entities;
namespace Droosy_DAL.Seeding;
public static class DemoSeedData
{
 public const string Version = "droosy-demo-v1-phases-1-3";
 public static readonly (string Id, string Name, DemoRole Role)[] Users = [
 ("T01", "أ. محمد عادل", DemoRole.Teacher), ("P01", "ولي أمر أحمد وياسين", DemoRole.Parent),
 ("S01", "أحمد محمود", DemoRole.Student), ("S02", "ياسين محمود", DemoRole.Student),
 ("S03", "عمر خالد", DemoRole.Student), ("S04", "زياد حسن", DemoRole.Student),
 ("S05", "مروان علي", DemoRole.Student), ("S06", "يوسف أحمد", DemoRole.Student),
 ("S07", "سليم طارق", DemoRole.Student), ("S08", "آدم سامح", DemoRole.Student) ];
 public static readonly (string Student, string Course, decimal Price, decimal Paid, int EndDays)[] Subscriptions = [
 ("S01","ALG",300,200,5), ("S03","ALG",300,300,20), ("S04","ALG",300,0,-1), ("S05","ALG",300,300,15),
 ("S02","GEO",250,250,2), ("S06","GEO",250,150,14), ("S07","GEO",250,0,-3), ("S08","GEO",250,250,20) ];
 public static readonly (string Course, string[] Units, string[] Lessons, string[] Descriptions)[] Content = [
 ("ALG", ["المعادلات البسيطة", "تطبيقات ومراجعة"],
 ["حل معادلة من خطوة واحدة", "حل معادلة من خطوتين", "مسائل لفظية على المعادلات", "مراجعة الجبر"],
 ["نتعرف على فكرة المعادلة وكيف نحافظ على توازن طرفيها، ثم نحل مثالًا من خطوة واحدة.", "نرتب خطوات الحل: نطرح العدد الثابت من الطرفين، ثم نقسم على معامل المجهول.", "نحوّل المسألة اللفظية إلى معادلة بسيطة، ثم نتحقق من صحة الحل.", "مراجعة شاملة على المعادلات مع أمثلة وتطبيقات متنوعة."]),
 ("GEO", ["الزوايا والمثلثات", "الأشكال والمراجعة"],
 ["الزوايا المتتامة والمتكاملة", "مجموع زوايا المثلث", "محيط ومساحة الأشكال", "مراجعة الهندسة"],
 ["نتعرف على الزوايا المتتامة التي مجموعها ٩٠° والمتكاملة التي مجموعها ١٨٠°.", "نستخدم مجموع زوايا المثلث ١٨٠° لإيجاد الزاوية المجهولة.", "نميز بين المحيط والمساحة ونطبق قوانين المستطيل والمربع.", "نراجع الزوايا والمثلثات والمحيط والمساحة قبل التقييم."]) ];
}
