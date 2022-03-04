using Google.Apis.Classroom.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace classroom_api.Services
{
    public class ClassRoomOAuth
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/classroom.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { ClassroomService.Scope.ClassroomCourses };
        static string ApplicationName = "Classroom API .NET Quickstart";
        public static ClassroomService GetClassroomService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials-desktop.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user", //от этой штуки зависит, с каким именем токен сохранится/считает
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Classroom API service.
            return new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            //// Define request parameters.
            //CoursesResource.ListRequest request = service.Courses.List();
            //request.PageSize = 10;

            //// List courses.
            //ListCoursesResponse response = request.Execute();
            //Console.WriteLine("Courses:");
            //if (response.Courses != null && response.Courses.Count > 0)
            //{
            //    foreach (var course in response.Courses)
            //    {
            //        Console.WriteLine("{0} ({1})", course.Name, course.Id);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No courses found.");
            //}
        }
    }
    

    
}
