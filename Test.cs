[Test]
public void StoreOnlyDuplicateDetailsFromRowsIntoCollectionOld()
{
    query = "SELECT * FROM EVENTLOG WHERE STATUS = 'S' AND RECORD_ID IN(85793,85790,85786,85787,85792,85785,85788,85789,85791)";
    var results = dc.newDbQuery(query);
    var eventLogObjects = new List<EventLogObj>();
    var duplicateEventLogObjects = new List<EventLogObj>();

    while (results.Read())
    {
        eventLogObj = new EventLogObj()
        {
            RecordId = (decimal)results.GetValue(0),
            TableKey = (string)results.GetValue(1),
            Status = (string)results.GetValue(2),
            EventTime = (DateTime)results.GetValue(4),
            Perpetrator = (string)results.GetValue(5)
        };

        var existing = eventLogObjects.Where(
            e => e.TableKey.ToLower().Equals(eventLogObj.TableKey.ToLower())
        ).ToList();

        if (existing.Count == 0)
        {
            eventLogObjects.Add(eventLogObj);
        }
        else
        {
            duplicateEventLogObjects.Add(eventLogObj);
        }

    }

    Assert.AreEqual(2, duplicateEventLogObjects.Count);
}

[Test]
public void StoreOnlyDuplicateDetailsFromRowsIntoCollection()
{
    var event1 = new EventLogObj() { RecordId = 1, TableKey = "PERSON_CODE=1", Status = "S" };
    var event2 = new EventLogObj() { RecordId = 2, TableKey = "PERSON_CODE=2", Status = "S" };
    var event3 = new EventLogObj() { RecordId = 3, TableKey = "PERSON_CODE=3", Status = "S" };
    var event4 = new EventLogObj() { RecordId = 4, TableKey = "PERSON_CODE=2", Status = "S" };
    var event5 = new EventLogObj() { RecordId = 5, TableKey = "PERSON_CODE=1", Status = "S" };

    var events = new List<EventLogObj>() { event1, event2, event3, event4, event5 };

    var mockEventsRepository = new Mock<IEventRepository>();

    mockEventsRepository.Setup(ev => ev.GetEvents())
        .Returns(events);

    mockEventsRepository.Setup(ev => ev.DeleteEvent(It.IsAny<decimal>()))
        .Callback((decimal RecID) =>
        {
            events.RemoveAll(e => e.RecordId == RecID);
        });

    // not using delegate
    //mockEventsRepository.Setup(ev => ev.Count())              
    //    .Returns(events.Count);

    // using delegate
    mockEventsRepository.Setup(ev => ev.Count())
        .Returns(() => { return events.Count; });

    IEventRepository EventsMockDatabase = mockEventsRepository.Object;

    var eventLogObjects = new List<EventLogObj>();
    var duplicateEventLogObjects = new List<EventLogObj>();

    foreach (EventLogObj elo in EventsMockDatabase.GetEvents())
    {
        var existing = eventLogObjects.Where(
            e => e.TableKey.Equals(elo.TableKey)
        ).ToList();

        if (existing.Count == 0)
        {
            eventLogObjects.Add(elo);
        }
        else
        {
            duplicateEventLogObjects.Add(elo);
        }
    }

    Assert.AreEqual(2, duplicateEventLogObjects.Count);
}
