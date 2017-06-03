function test(title) {
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

                var replaceOptions = {};
                replaceOptions.indexAction = 'default';
                replaceOptions.etag = roomReservation._etag;
                /*
                for(var i=0 ; i<10000 ; i++)
                {
	                for(var n=0 ; n<10 ; n++) {
						roomReservation.Title = title + i;
					}
				}
				*/
                var accept = collection.replaceDocument(roomReservation._self, roomReservation, replaceOptions,
                function (err, docReplaced) {
                    if (err) throw err;
                });

                for(var i=0 ; i<1000000 ; i++)
                {
	                for(var n=0 ; n<1 ; n++)
		                roomReservation.Title = title;
				}

            });
}
