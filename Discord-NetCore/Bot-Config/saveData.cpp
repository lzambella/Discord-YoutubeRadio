#include "saveData.h"
#include <string>
#include <iostream>
#include <fstream>
#include <QtCore\qxmlstream.h>
#include <QtCore\qfile.h>
saveData::saveData() 
{

}
saveData::saveData(QString discord, QString facebook, QString wolfram, QString owner, QString database)
{
	discordToken = discord;
	wolframToken = wolfram;
	databaseToken = database;
	ownerId = owner;
	facebookToken = facebook;
}
int saveData::generateXml() {
	try {
		QFile file("config.xml");
		file.open(QIODevice::ReadWrite);

		QXmlStreamWriter writer;

		writer.setDevice(&file);
		writer.writeStartDocument();
		writer.writeStartElement("config");

		writer.writeAttribute("discordToken", discordToken);
		writer.writeAttribute("wolframToken", wolframToken);
		writer.writeAttribute("facebookToken", facebookToken);
		writer.writeAttribute("ownerId", ownerId);
		writer.writeAttribute("databaseToken", databaseToken);

		writer.writeEndElement();
		writer.writeEndDocument();
		
		file.close();
		return 0;
	}
	catch (int e) {
		return -1;
	}
}


saveData::~saveData()
{
}
