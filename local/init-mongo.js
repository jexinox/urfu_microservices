db = new Mongo().getDB("notifications");

db.createCollection('notifications', { capped: false });

db.createUser({
    user: 'ntf',
    pwd: 'ntf',
    roles: [
        {
            role: 'readWrite',
            db: 'notifications',
        },
    ],
});