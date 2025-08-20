
CREATE database workingcalendar;
\c workingcalendar;
CREATE user workingcalendar ;
ALTER role workingcalendar SUPERUSER;
ALTER user workingcalendar with encrypted password 'workingcalendar';
GRANT all privileges on database workingcalendar to workingcalendar;


CREATE TABLE workingcalendar (
    id SERIAL primary key,
    dateday date NULL,
    isworkingday bit NULL
);



INSERT INTO workingcalendar ( dateday, isworkingday )
VALUES
  (now(), '1'),
  (now(), '1'),
  (now(), '1'),
  (now(), '1'),
  (now(), '1'),
  (now(), '1'),
  (now(), '1'),
  (now(), '1');



