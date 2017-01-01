#pragma once
#include <string>
#include <QtCore\qstring.h>
class saveData
{
public:
	saveData();
	saveData(QString discord, QString facebook, QString wolfram, QString owner, QString database);
	~saveData();
	int generateXml();
	QString discordToken, facebookToken, wolframToken, databaseToken, ownerId;
};

