namespace P01_HospitalDatabase
{
    using Microsoft.EntityFrameworkCore;
    using P01_HospitalDatabase.Data;
    using P01_HospitalDatabase.Data.Models;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new HospitalContext())
            {
                var doctor = new Doctor();
                doctor.Name = "Doctor Radeva";
                doctor.Specialty = "Voda";

                var visitation = context.Visitations.First();
                visitation.DoctorId = doctor.DoctorId;

                context.Doctors.Add(doctor);

                context.SaveChanges();
            }
        }
    }
}
