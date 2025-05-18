using MovieLibrary.Models;

namespace MovieLibrary.Data;

public class BorrowRecordRepository
{
    public IEnumerable<BorrowRecord> GetAllBorrowRecord()
    {
        return Repository.Instance.BorrowRecords;
    }
}