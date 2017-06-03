function test2(title) {
    var context = getContext();
    var collection = context.getCollection();
    var response = context.getResponse();
	
	var roomReservation;
	
	var filterQuery = 'SELECT * FROM Reservations r where r.id  = "0000000001"';
	response.Body = "succeeded";

    var accept = collection.queryDocuments(collection.getSelfLink(), filterQuery, {},
            function (err, documents, responseOptions) {
                if (err) throw new Error("Error!!1!" + err.message);

                if (documents.length != 1) throw "Unable to find both names " + documents.length;
                roomReservation = documents[0];
                response.setBody(roomReservation.Title);

                var options = {};
                options.etag = roomReservation._etag;

                for(var i=0 ; i<1000000 ; i++)
                {
	                for(var n=0 ; n<100 ; n++)
		                roomReservation.Title = title;
				}
                var accept = collection.replaceDocument(roomReservation._self, roomReservation, options,
                function (err, docReplaced) {
                    if (err) throw "Unable to update player 1, abort ";
                });
                

            });
}
