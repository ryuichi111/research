function bulkDeletReserve(fromDate, toDate) {
    var context = getContext();
    var collection = context.getCollection();
    var response = context.getResponse();
	
	var filterQuery = 'SELECT * FROM Reservations r where r.Start >= "' + fromDate + '" and r.Start < "' + toDate + '"';
    var accept = collection.queryDocuments(collection.getSelfLink(), filterQuery, {},
            function (err, documents, responseOptions) {
				for(var i=0 ; i<documents.length ; i++){
					var accept = collection.deleteDocument(documents[i]._self, {},
						function (err, documents, responseOptions) {
						});
				}
                response.setBody(documents.length);
            });
/*	var roomReservation;
	var sDate = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDay());
	sDate.setHours(startHour);
	
	response.setBody(sDate);
	*/
}
