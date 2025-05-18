using MovieLibrary.Data;
using MovieLibrary.Models;

namespace MovieLibrary.Services;

public class BorrowReturnService
{
	private static BorrowReturnService? _instance;
	public static BorrowReturnService Instance => _instance ??= new BorrowReturnService(Repository.Instance.MovieRepo);

	private readonly MovieRepository _movieRepository;

	public BorrowReturnService(MovieRepository movieRepository)
	{
		_movieRepository = movieRepository;

	}

	public string BorrowMovie(string movieId, User user)
	{
		var movie = _movieRepository.GetMovieById(movieId);
		if (movie == null)
		{
			throw new Exception("Movie not found");
		}

		if (Repository.Instance.BorrowRecords.Any(br => br.MovieId == movieId && br.UserId == user.Id))
		{
			return $"User '{user.Name}' has already borrowed this movie.";
		}

		if (movie.IsAvailable)
		{
			movie.IsAvailable = false;
			var borrowRecord = new BorrowRecord
			{
				Id = Repository.Instance.GenerateNewRecordId(),
				MovieId = movieId,
				MovieTitle = movie.Title,
				UserId = user.Id!,
				UserName = user.Name,
				BorrowDate = DateTime.Now.ToShortDateString(),
				ReturnDate = null
			};

			Repository.Instance.BorrowRecords.Add(borrowRecord);

			if (user.MovieViewCount.ContainsKey(movieId))
			{
				user.MovieViewCount[movieId]++;
			}
			else
			{
				user.MovieViewCount.Add(movieId, 1);
			}

			return $"Movie '{movie.Title}' is now borrowed by {user.Name}.";
		}
		else
		{
			// Prevent duplicate entries in waiting list
			if (!string.IsNullOrEmpty(user.Id) && !movie.WaitingList.Contains(user.Id))
			{
				movie.WaitingList.Enqueue(user.Id);
				return $"Movie '{movie.Title}' is currently unavailable. You have been added to the waiting list.";
			}
			else if (string.IsNullOrEmpty(user.Id))
			{
				return "Invalid user ID.";
			}
			else
			{
				return $"You are already in the waiting list for '{movie.Title}'.";
			}
		}
	}

	public string ReturnMovie(string movieId, User returningUser)
	{
		var movie = _movieRepository.GetMovieById(movieId);
		if (movie == null)
		{
			return "Movie not found";
		}

		var borrowRecord = Repository.Instance.BorrowRecords.FirstOrDefault(br => br.MovieId == movieId && br.UserId == returningUser.Id && br.IsActive);
		if (borrowRecord == null)
		{
			return "This user has not borrowed this movie.";
		}

		// Update return date
		borrowRecord.ReturnDate = DateTime.Now.ToShortDateString();
		movie.IsAvailable = true;
		borrowRecord.IsActive = false;

		// Remove the borrow record
		//Repository.Instance.BorrowRecords.Remove(borrowRecord);

		if (movie.WaitingList.Count > 0)
		{
			var nextUserId = movie.WaitingList.Dequeue();
			movie.IsAvailable = false;
			var nextUser = Repository.Instance.UserRepo.GetUserById(nextUserId);

			var newRecord = new BorrowRecord
			{
				Id = Repository.Instance.GenerateNewRecordId(),
				MovieId = movieId,
				MovieTitle = movie.Title,
				UserId = nextUserId,
				UserName = nextUser!.Name,
				BorrowDate = DateTime.Now.ToShortDateString(),
				ReturnDate = null
			};
			Repository.Instance.BorrowRecords.Add(newRecord);

			// Update view count for next user
			if (nextUser != null)
			{
				if (nextUser.MovieViewCount.ContainsKey(movieId))
					nextUser.MovieViewCount[movieId]++;
				else
					nextUser.MovieViewCount.Add(movieId, 1);
			}

			return $"Movie '{movie.Title}' has been returned by {returningUser.Name} and automatically issued to next waiting user (User Id : {nextUserId}).";
		}

		return $"Movie '{movie.Title}' has been returned by {returningUser.Name} and is now available for borrowing.";
	}
}