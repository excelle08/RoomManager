USE `room_manager`;

INSERT INTO users (username, password, privilege) values('admin', md5('admin'), 2);
INSERT INTO users (username, password, privilege) values('staff', md5('123456'), 2);

