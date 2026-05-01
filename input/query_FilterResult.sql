SELECT StudentResult.ResultId, StudentResult.StudentId, StudentResult.TestId, StudentResult.Mark
FROM StudentResult
WHERE (((StudentResult.TestId)=[forms]![ChooseTest]![lstTest]));

