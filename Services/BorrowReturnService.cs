using MovieLibrary.Data;
using MovieLibrary.Models;

namespace MovieLibrary.Services;

public class BorrowReturnService
{
    private readonly MovieRepository _movieRepository;
    public List<BorrowRecord> BorrowRecords { get; set; } = new();

    public BorrowReturnService(MovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public string BorrowMovie(string id, User user)
    {
        var movie = _movieRepository.GetMovieById(id);
        if (movie == null)
        {
            throw new Exception("Movie not found");
        }

        if (movie.IsAvailable)
        {
            movie.IsAvailable = false;
            var borrowRecord = new BorrowRecord
            {
                MovieId = id,
                UserId = user.Id,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.MinValue
            };

            BorrowRecords.Add(borrowRecord);

            if (user.MovieViewCount.ContainsKey(id))
            {
                user.MovieViewCount[id]++;
            }
            else
            {
                user.MovieViewCount.Add(id, 1);
            }

            return $"Movie '{movie.Title}' is now borrowed by {user.Name}.";

        }
        else
        {
            movie.WaitingList.Enqueue(user.Id!);
            return $"Movie '{movie.Title}' is currently unavailable. You have been added to the waiting list.";
        }
    }

    public string ReturnMovie(string id, User returningUser)
    {
        var movie = _movieRepository.GetMovieById(id);
        if (movie == null)
        {
            return "Movie not found";
        }

        var borrowRecord = BorrowRecords.FirstOrDefault(br => br.MovieId == id && br.UserId == returningUser.Id);
        if (borrowRecord == null)
        {
            return "This user has not borrowed this movie.";
        }

        movie.IsAvailable = true;
        foreach (var record in BorrowRecords)
        {
            if (record.MovieId == id && record.UserId == returningUser.Id)
            {
                BorrowRecords.Remove(record);
                break;
            }
        }

        if (movie.WaitingList.Count > 0)
        {
            var nextUserId = movie.WaitingList.Dequeue();
            movie.IsAvailable = false;
            BorrowRecord record = new BorrowRecord
            {
                Id = Repository.Instance.GenerateNewRecordId(),
                MovieId = id,
                UserId = nextUserId,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.MinValue
            };

            BorrowRecords.Add(record);

            // Notify the next user in the waiting list
            return $"Movie '{movie.Title}' has been returned by {returningUser.Name} and automatically issued to next waiting user (User Id : {nextUserId}).";
        }

        return $"Movie '{movie.Title}' has been returned by {returningUser.Name} and is now available for borrowing.";
    }
}