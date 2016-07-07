/**
 * Created by Excelle on 7/7/16.
 */

$(document).ready(function () {
    getApi('/api/user', {}, function (err, r) {
        if (err || !r || r.privilege != 2) {
            location.assign('/');
        }
    });
});


$(document).ready(function () {
    getApi('/api/roomtype', {}, function (err, r) {
        if (err) {
            alert('Error getting the room types. ' + err.message);
        } else {
            getApi('/api/room', {
                page: 1,
                limit: 20
            }, function (err2, s) {
                if (err2) {
                    alert('Error retrieving rooms: ' + err.message );
                } else {
                    roomView(r, s);
                }
            });
        }
    });
});

function roomView(roomtype, rooms) {
    var vm = new Vue({
        el: '#room',
        data: {
            types: roomtype,
            rooms: rooms,
            category_name: '',
            category_price: '',
            room_name: '',
            room_num: '',
            room_type: '',
            room_price: '',
            room_capacity: '',
            n_room_id: '',
            n_room_name: '',
            n_room_num: '',
            n_room_type: '',
            n_room_price: '',
            n_room_capacity: '',
            n_room_status: '',
            n_type_id: '',
            n_type_name: '',
            n_type_price: '',
            page: 1
        },
        methods: {
            getStatus: function (val) {
                switch (val) {
                    case 0:
                        return '空闲';
                    break;

                    case 1:
                        return '已预订';
                    break;

                    case 2:
                        return '已入住';
                    break;

                    case 3:
                        return '维护中';
                    break;

                    default:
                        return 'N/a';
                    break;
                }
            },

            updateType: function () {
                var types = this.types;
                getApi('/api/roomtype', {}, function (err, r) {
                    if (err) {
                        alert('Error retrieving room type: ' + err.message);
                    } else {
                        types.splice(0, types.length);
                        for(var i in r) {
                            types.push(r[i]);
                        }
                    }
                });
            },

            updateRoom: function () {
                var rooms = this.rooms;
                getApi('/api/room', {
                    page: this.page,
                    limit: 20
                }, function (err, r) {
                    if (err) {
                        alert('Error retrieving rooms: ' + err.message);
                    } else {
                        rooms.splice(0, rooms.length);
                        for(var i in r) {
                            rooms.push(r[i]);
                        }
                    }
                });
            },

            deleteType: function (t) {
                var _this = this;
                if (confirm("你确定要删除此条分类[" + t.name + "]吗?")) {
                    deleteApi('/api/roomtype/' + t.id, {}, function (err, r) {
                        if (err) {
                            alert('Error deleting type.' + err.message);
                        } else {
                            _this.updateType();
                        }
                    });
                }
            },

            deleteRoom: function (rm) {
                if (confirm("你确定要删除此房间吗?")) {
                    deleteApi('/api/room/' + rm.id, {}, function (err, r) {
                        if (err) {
                            alert("Error deleting room: " + err.message);
                        } else {
                            this.updateRoom();
                        }
                    });
                }
            },
            
            showEditRoom: function (rm) {
                this.n_room_id = rm.id;
                this.n_room_name = rm.name;
                this.n_room_type = rm.type;
                this.n_room_num = rm.number;
                this.n_room_price = rm.custom_Price;
                this.n_room_capacity = rm.capacity;
                this.n_room_status = rm.status;
                $('#room-edit').modal('show');
            },

            showEditType: function (t) {
                this.n_type_id = t.id;
                this.n_type_name = t.name;
                this.n_type_price = t.typical_Price;
                $('#roomtype-edit').modal('show');
            },
            
            addRoom: function () {
                var _this = this;
                putApi('/api/room', {
                    name: this.room_name,
                    number: this.room_num,
                    type: this.room_type,
                    custom_Price: this.room_price,
                    capacity: this.room_capacity
                }, function (err, r) {
                    if (err) {
                        alert('Error adding room');
                    } else {
                        _this.updateRoom();
                    }
                });
            },

            addRoomType: function () {
                var _this = this;
                putApi('/api/roomtype', {
                    name: this.category_name,
                    typical_Price: this.category_price
                }, function (err, r) {
                    if (err) {
                        alert('Error adding room type - ' + err.message);
                    } else {
                        _this.updateType();
                    }
                });
            },

            editroom: function () {
                var _this = this;
                var dat = {
                    name: this.n_room_name,
                    number: parseInt(this.n_room_num),
                    type: parseInt(this.n_room_type),
                    custom_Price: parseFloat(this.n_room_price),
                    capacity: parseInt(this.n_room_capacity),
                    status: parseInt(this.n_room_status)
                };
                postApi('/api/room/' + this.n_room_id, dat, function (err, r) {
                    if (err) {
                        alert('Error editing room: ' + err.message);
                    } else {
                        _this.updateRoom();
                        $('#roomtype-edit').modal('hide');
                    }
                });
            },

            edittype: function () {
                var _this = this;
                var data = {
                    name: this.n_type_name,
                    typical_Price: parseFloat(this.n_type_price)
                };
                postApi('/api/roomtype/' + this.n_type_id, data, function (err, r) {
                    if (err) {
                        alert('Error editing roomtype: ' + err.message);
                    } else {
                        _this.updateType();
                        $('#roomtype-edit').modal('hide');

                    }
                });
            }
        }
    });
}

$(document).ready(function () {
    getApi('/api/item', {
        page: 1,
        limit: 20
    }, function (err, r) {
        if (err) {
            alert('Error loading items' + err.message);
        } else {
            itemView(r);
        }
    })
});

function itemView(items) {
    var vm = new Vue({
        el: '#item',
        data: {
            items: items,
            page: 1,
            item_id: 0,
            item_name: '',
            item_price: '',
            item_desc: '',
            n_item_name:'',
            n_item_price: '',
            n_item_desc: ''
        },
        methods: {
            update: function () {
                var items = this.items;
                getApi('/api/item', {
                    page: this.page,
                    limit: 20
                }, function (err, r) {
                    if (err) {
                        alert('Error retrieving items - ' + err.message);
                    } else {
                        items.splice(0, items.length);
                        for(var i in r) {
                            items.push(r[i]);
                        }
                    }
                });
            },
            appendItem: function () {
                var data = {
                    name: this.n_item_name,
                    typical_Price: parseFloat(this.n_item_price),
                    description: this.n_item_desc
                };
                var _this = this;

                putApi('/api/item', data, function (err, r) {
                    if (err) {
                        alert('Failed to append item: ' + err.message);
                    } else {
                        _this.update();
                    }
                })
            },

            showEditItem: function (item) {
                this.item_id = item.id;
                this.item_name = item.name;
                this.item_price = item.typical_Price;
                this.item_desc = item.description;

                $('#item-edit').modal('show');
            },

            editItem: function () {
                var data = {
                    id: this.item_id,
                    name: this.item_name,
                    typical_Price: parseFloat(this.item_price),
                    description: this.item_desc
                };
                var _this = this;

                postApi('/api/item', data, function (err, r) {
                    if (err) {
                        alert('Failed to edit item: ' + err.message);
                    } else {
                        _this.update();
                        $('#item-edit').modal('hide');
                    }
                })
            },

            deleteItem: function(item) {
                if(confirm('你确实要删除此商品[' + item.name + ']吗?')) {
                    var _this = this;
                    deleteApi('/api/item/' + item.id, {}, function (err, r) {
                        if (err) {
                            alert('Failed to delete: ' + err.message);
                        } else {
                            _this.update();
                        }
                    });
                }
            }
        }
    });
}

$(document).ready(function () {
    getApi('/api/user/all', {}, function (err, r) {
        if (err) {
            alert('Error retrieving user list.' + err.message);
        } else {
            userView(r);
        }
    })
});

function userView(users) {
    var vm = new Vue({
        el: '#user',
        data: {
            users: users,
            n_username: '',
            n_password: '',
            n_privilege: 1,
            uid: 0,
            username: '',
            password: '',
            privilege: 1
        },
        methods: {
            createUser: function () {
                var data = {
                    userName: this.n_username,
                    password: this.n_password,
                    privilege: parseInt(this.n_privilege)
                };
                var users = this.users;

                putApi('/api/user', data, function(err, r) {
                    if (err) {
                        alert('Failed to create user: ' + err.message);
                    } else {
                        users.push(r);
                    }
                });
            },

            showEditUserPasswd: function (u) {
                uid = u.id;
                $('#user-resetpwd').modal('show');
            },

            showEditPrivilege: function (u) {
                uid = u.uid;
                $('#user-edit-privilege').modal('show');
            },

            removeUser: function (u) {
                var users = this.users;
                deleteApi('/api/user/' + u.id, {}, function (err, r) {
                    if (err) {
                        alert('Failed to delete user: ' + err.message);
                    } else {
                        for(var i in users) {
                            if (users[i].id == u.id) {
                                users.splice(i, 1);
                            }
                        }
                    }
                });
            },

            resetpwd: function () {
                var uid = this.uid;
                var pwd = this.password;
                patchApi('/api/user/' + uid + '/password', pwd, function (err, r) {
                    if (err) {
                        alert('Failed to change password. ' + err.message);
                    } else {
                        $('#user-resetpwd').modal('hide');
                    }
                });
            },
            
            
        }
    })
}