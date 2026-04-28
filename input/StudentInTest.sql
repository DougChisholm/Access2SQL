SELECT FilterResult.ResultId, FilterResult.StudentId, Student.StudentName, FilterResult.TestId, FilterResult.Mark
FROM Student LEFT JOIN FilterResult ON Student.StudentId = FilterResult.StudentId
ORDER BY Student.StudentName;

