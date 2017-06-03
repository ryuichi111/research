function calcTimeUdf(start, end)
{
	var startDate = Date.parse(start);
	var endDate = Date.parse(end);
	
	return (endDate - startDate) / 1000 / 60;
}
