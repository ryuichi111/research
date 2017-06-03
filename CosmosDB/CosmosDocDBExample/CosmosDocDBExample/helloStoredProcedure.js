function helloStoredProcedure(yourName) {
  var context = getContext();
  var response = context.getResponse();
  
  response.setBody("Hello!! " + yourName);
}
