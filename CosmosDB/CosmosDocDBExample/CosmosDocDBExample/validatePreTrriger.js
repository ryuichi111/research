function validatePreTrriger()
{
	var context = getContext();
	var request = context.getRequest();

	var createdDocument = request.getBody();
	
	var now = new Date(); // GMT時間
	now.setHours( now.getHours() + 9); // GMT+9hでTokyo時間
	if( Date.parse(createdDocument.Start) < Date.parse(now) )
		throw '過去の予約を取ることはできません。';

	createdDocument.Title = createdDocument.Title + ' [検証OK]';
	createdDocument.AppendInfo = 'プリトリガーで追加';
	
	request.setBody(createdDocument);
}
