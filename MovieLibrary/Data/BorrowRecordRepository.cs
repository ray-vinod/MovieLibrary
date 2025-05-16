using MovieLibrary.Models;

namespace MovieLibrary.Data;

public class BorrowRecordRepository
{
    private readonly LinkedList<BorrowRecord> _records = new();

    public IEnumerable<BorrowRecord> GetAllBorrowRecord()
    {
        return _records;
    }
}