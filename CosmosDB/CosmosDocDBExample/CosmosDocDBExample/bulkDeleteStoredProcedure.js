function bulkDeleteStoredProcedure(fromDate, toDate) {
    var context = getContext();
    var collection = context.getCollection();
    var response = context.getResponse();
	
	var filterQuery = 'SELECT * FROM Reservations r where r.Start >= "' + fromDate + '" and r.Start < "' + toDate + '"';
    var accept = collection.queryDocuments(collection.getSelfLink(), filterQuery, {},
            function (error, documents, responseOptions) {
				
				if (error) throw error;
				
				for(var i=0 ; i<documents.length ; i++){
					var accept = collection.deleteDocument(documents[i]._self, {},
						function (error, documents, responseOptions) {
							if (error) throw error;
						});
				}
                response.setBody(documents.length);
            });
}
