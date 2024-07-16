using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace ExerciseTracker;
public class ExerciseService : IExerciseService
{
    private readonly IUserInput _userInput;
    private readonly ExerciseController _exerciseController;
    public ExerciseService(ExerciseController controller, IUserInput userInput)
    {
        _userInput = userInput;
        _exerciseController = controller;
    }
    public void MainMenu()
    {
        Console.Clear();
        var option = AnsiConsole.Prompt(new SelectionPrompt<Menu>()
        .Title("Choose")
        .AddChoices(
            Menu.ViewExercise,
            Menu.ViewAllExercises,
            Menu.AddExercise,
            Menu.DeleteExercise,
            Menu.UpdateExercise,
            Menu.Quit));

        switch (option)
        {
            case Menu.ViewExercise:
                GetExerciseById();
                break;
            case Menu.ViewAllExercises:
                GetExercises();
                break;
            case Menu.AddExercise:
                AddExercise();
                break;
            case Menu.DeleteExercise:
                DeleteExercise();
                break;
            case Menu.UpdateExercise:
                UpdateExercise();
                break;
            case Menu.Quit:
                Environment.Exit(0);
                break;
        }

    }
    public void AddExercise()
    {
        var exercise = _userInput.GetExerciseInfo();
        _exerciseController.AddExercise(exercise);
        Console.WriteLine("Record successfully added.");
    }

    public void DeleteExercise()
    {
        var exercises = _exerciseController.GetExercises();
        if (exercises.Count() > 0)
        {
            GetExercises(1);
            var id = _userInput.GetExerciseId("Choose the id of the record you want to delete:", exercises);
            if (id == 0) MainMenu();
            else
            {
                _exerciseController.DeleteExercise(id);
                Console.WriteLine("Record deleted.");
            }
        }
        else
        {
            Console.WriteLine("No records in database.");
            Console.WriteLine("Press enter to go back to main menu.");
            Console.ReadLine();
        }
    }

    public void GetExerciseById()
    {
        var exercises = _exerciseController.GetExercises();
        if (exercises.Count() < 1)
        {
            Console.WriteLine("No records in database. Add a record first!");
            Console.WriteLine("Press enter to go back to main menu.");
            Console.ReadLine();
            MainMenu();
        }
        else
        {
            var id = _userInput.GetExerciseId("Choose the id of the record you want to view:", exercises);
            if (id == 0) MainMenu();
            else
            {
                var exercise = _exerciseController.GetExerciseById(id);
                var panel = new Panel($@"
Id: {exercise.Id}
Start Date: {exercise.StartDate}
End Date: {exercise.EndDate}
Duration: {exercise.Duration}
Comments: {exercise.Comments}");
                panel.Header("Exercise");
                panel.Padding(2, 2, 2, 2);
                AnsiConsole.Write(panel);
                Console.WriteLine("Press enter to go back to main menu.");
                Console.ReadLine();
            }
        }

    }

    public void GetExercises(int num = 0)
    {
        var exercises = _exerciseController.GetExercises();
        if (exercises.Count() > 0)
        {
            Spectre.Console.Table table = new Spectre.Console.Table();
            table.AddColumns("Id", "Start Date", "End Date", "Duration", "Comments");
            foreach (var exercise in exercises)
            {
                table.AddRow($"{exercise.Id}", $"{exercise.StartDate}", $"{exercise.EndDate}", $"{exercise.Duration}", $"{exercise.Comments}");
            }
            AnsiConsole.Write(table);
            if (num == 0)
            {
                Console.WriteLine("Press enter to go back to main menu.");
                Console.ReadLine();
            }
        }
        else
        {
            Console.WriteLine("No records in database.");
            Console.WriteLine("Press enter to go back to main menu.");
            Console.ReadLine();
        }

    }
    public void UpdateExercise()
    {
        var exercise = new Exercise();
        var exercises = _exerciseController.GetExercises();
        GetExercises(1);
        if (exercises.Count() > 0)
        {
            exercise.Id = _userInput.GetExerciseId("Choose the id of the record you want to update:", exercises);
            if (exercise.Id == 0) MainMenu();
            else
            {
                var updatedInfo = _userInput.GetExerciseInfo();
                exercise.StartDate = updatedInfo.StartDate;
                exercise.EndDate = updatedInfo.EndDate;
                exercise.Duration = updatedInfo.Duration;
                exercise.Comments = updatedInfo.Comments;
                _exerciseController.UpdateExercise(exercise);
                Console.WriteLine("Successfully updated.");
            }
        }
    }
}