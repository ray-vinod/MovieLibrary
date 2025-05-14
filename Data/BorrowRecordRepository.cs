using System.Collections;
using MovieLibrary.Models;

namespace MovieLibrary.Data;

public class BorrowRecordRepository
{
    // Using a Hashtable for fast lookups by ID
    private readonly Hashtable _recordLookup = new();

    // LinkedList to maintain the order of users
    private readonly LinkedList<BorrowRecord> _records = new();

    public IEnumerable<BorrowRecord> GetAllBorrowRecord()
    {
        return _records;
    }
}