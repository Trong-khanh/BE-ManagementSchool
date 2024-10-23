namespace ManagementSchool.Entities
{
    public class Semester
    {
        public int SemesterId { get; set; }
        public SemesterType SemesterType { get; set; } // Ensure this is of type SemesterType
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AcademicYear { get; set; }
        public ICollection<Score> Scores { get; set; }
        public ICollection<ClassSemester> ClassSemesters { get; set; }
    }
}