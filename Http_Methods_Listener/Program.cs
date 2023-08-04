using Http_Methods_Listener;
using System.Net;
using System.Text.Json;

List<User> users = new() { new User(1, "user1", "user11", 11), new User(2, "user2", "user22", 22), new User(3, "user3", "user33", 33) };
var listener = new HttpListener();

listener.Prefixes.Add(@"http://localhost:27002/");

listener.Start();

while (true)
{
    var context = await listener.GetContextAsync();
    var request = context.Request;
    var response = context.Response;
    Console.WriteLine("Connected");
    if (request.HttpMethod == HttpMethod.Get.Method)
    {
        if (request.HasEntityBody)
        {
            var stream = request.InputStream;
            var streader = new StreamReader(stream);
            var id=int.Parse(await streader.ReadToEndAsync());
            var user = users.FirstOrDefault(u => u.Id == id);
            var writer=new StreamWriter(response.OutputStream);
            if (user != null)
            {
                writer.Write(JsonSerializer.Serialize(user));
                writer.Close();

            }
            else
            {
                response.StatusCode = 404;
                writer.Write("Incorrect Id");
                writer.Close();
            }
        }
        else
        {
            StreamWriter streamWriter = new StreamWriter(response.OutputStream);
            streamWriter.Write(JsonSerializer.Serialize(users));
            streamWriter.Close();
        }
    }
    else if(request.HttpMethod == HttpMethod.Post.Method)
    {
        var stream = request.InputStream;
        var reader = new StreamReader(stream);
        var user = JsonSerializer.Deserialize<User>(await reader.ReadToEndAsync());
        string answer=string.Empty;
        if (user is not null) {
            users.Add(user);
            answer = "User Added";
        }
        else answer = "User Did not added";
        StreamWriter writer = new StreamWriter(response.OutputStream);
        writer.Write(answer);
        writer.Close();
    }
    else if (request.HttpMethod == HttpMethod.Put.Method)
    {
        var stream = request.InputStream;
        var reader = new StreamReader(stream);
        var user = JsonSerializer.Deserialize<User>(await reader.ReadToEndAsync());
        string answer = string.Empty;
        if (user is not null)
        {
            var changeduser=users.FirstOrDefault(u=>u.Id==user.Id);
            if (changeduser is not null)
            {
                var index = users.IndexOf(changeduser);
                users[index] = user;
                answer = "User Changed";
            }
            else answer = "Incorrect Id";
        }
        else
        answer = "User Did not added";
        StreamWriter writer = new StreamWriter(response.OutputStream);
        writer.Write(answer);
        writer.Close();
    }
    else if(request.HttpMethod == HttpMethod.Delete.Method)
    {
        var stream = request.InputStream;
        StreamReader sr=new StreamReader(stream);
        var id = int.Parse(await sr.ReadToEndAsync());
        var answer = string.Empty;
        var deleteduser = users.FirstOrDefault(u => u.Id == id);
        if (deleteduser is not null)
        {
            users.Remove(deleteduser);
            answer = "User Deleted";
        }
        else answer = "Incorrect Id";
        StreamWriter writer=new StreamWriter(response.OutputStream);
        await writer.WriteAsync(answer);
        writer.Close();
    }
}