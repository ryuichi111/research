function postTriggerExample()
{
  var context = getContext();
  var collection = context.getCollection();
  var response = context.getResponse();
  
  var createdDocument = response.getBody();

  var filterQuery = 'SELECT * FROM root r WHERE r.id = "_metadata"';
  var accept = collection.queryDocuments(collection.getSelfLink(), filterQuery,
    function(err, documents, responseOptions)
    {
      if(err) throw err;
      
      if( documents.length == 0 ) {
        var metadata = {};
        metadata.id = '_metadata';
        metadata.Room = createdDocument.Room;
        metadata.Count = 0;
        collection.createDocument(collection.getSelfLink(), metadata, {},
        function(err, document, responseOptions)
        {
          if(err) throw err;
          
          var metadataDocument = document;
          countUpDoucumentCount(metadataDocument);
        });
      }
      else 
      {
        var metadataDocument = documents[0];
        countUpDoucumentCount(metadataDocument);

      }
    });

  if(!accept) throw "Unable to update metadata, abort";

  function countUpDoucumentCount(metadataDocument)
  {
     metadataDocument.Count += 1;
     var accept = collection.replaceDocument(metadataDocument._self,
         metadataDocument, 
         function(err, docReplaced) {
            if(err) err;
         });
     if(!accept) throw err;
  }
}
