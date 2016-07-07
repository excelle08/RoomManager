$(document).ready(function() {
    getApi('/api/room', {
        page: 1,
        limit: 40
    }, function(err, r) {
        if (err) {
            alert("获取房间列表错误： " + err.message);
        } else {
            getApi('/api/room/counts', function (err, c) {
                var res;
                if (err) {
                    res = {
                        vacant: 0,
                        reserved: 0,
                        checkedin: 0,
                        in_Maintenance: 0
                    };
                } else {
                    res = c;
                }
                roomlistView(r, c);
            })
        }
    });
});

function roomlistView(room, count) {
    var vm = new Vue({
        el: "#room",
        data: {
            rooms: room,
            room: {},
            customers: [],
            page: 1,
            specific: '',
            keyword: '',
            counts: count
        },
        methods: {
            getStatusClass: function (room) {
                switch(room.status) {
                    case 0:
                    return "btn-success";
                    break;

                    case 1:
                    return "btn-warning";
                    break;

                    case 2:
                    return "btn-danger";
                    break;

                    case 3:
                    return "btn-info";
                    break;

                    default:
                    return "btn-default";
                    break;
                }
            },

            showRoom: function (selected_room) {
                this.room = {
                    id: selected_room.id,
                    name: selected_room.name,
                    type: selected_room.type,
                    typename: '',
                    custom_Price: selected_room.custom_Price,
                    description: selected_room.description,
                    price: 0,
                    status: selected_room.status,
                    capacity: selected_room.capacity,
                    number: selected_room.number
                };
                var rm = this.room;
                var cs = this.customers;
                getApi('/api/roomtype/' + selected_room.type, function (err, r) {
                    if (err) {
                        alert('获取房间类型出错: ' + err.message);
                    } else {
                        rm.typename = r.name;
                        rm.price = (rm.custom_Price == null || rm.custom_Price - 1e4 < 0) ? r.typical_Price : rm.custom_Price;

                        if (rm.status != 0) {
                            getApi('/api/room/' + rm.id.toString() + '/customers', function (err2, res) {
                                if (err2) {
                                    alert('获取客人信息出错: ' + err.message);
                                } else {
                                    cs.splice(0, cs.length);
                                    for (var i in res) {
                                        cs.push(res[i]);
                                    }
                                    $('#room-detail').modal('show');
                                }
                            });
                        } else {
                            $('#room-detail').modal('show');
                        }
                    }
                });
            },

            checkIn: function (customer) {
                var customers = this.customers;
                postApi('/api/room/' + customer.room_Id + '/customer', customer,
                function (err, r) {
                    if (err) {
                        alert('入住操作错误: ' + err.message);
                    } else {
                        for(var i in customers) {
                            if (customer[i].id == r.id) {
                                customer[i] = r;
                            }
                        }
                        alert('入住成功');
                    }
                });
            },

            update: function () {
                var rooms = this.rooms;
                getApi('/api/room', {
                    page: this.page,
                    limit: 40,
                    keyword: this.keyword,
                    specific: this.specific
                }, function (err, r) {
                    if (err) {
                        alert("获取房间列表错误: " + err.message);
                    } else {
                        rooms.splice(0, rooms.length);
                        for(var i in r) {
                            rooms.push(r[i]);
                        }
                    }
                });
            },

            clearSpecific: function () {
                this.specific = '';
                this.update();
            },

            getEmpties: function () {
                this.specific = 'vacant';
                this.update();
            },

            getReserved: function () {
                this.specific = 'reserved';
                this.update();
            },

            getCheckedin: function () {
                this.specific = 'checkedin';
                this.update();
            },

            getMaintain: function () {
                this.specific = 'inmaintenance';
                this.update();
            },

            reservation: function (room) {
                location.assign('/reservation.html?room=' + room.id);
            },

            getStatus: function(status) {
                switch(status) {
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
                        return '(无效状态)';
                        break;
                }
            },

            toDateTime: function (ts) {
                return toDateTime(ts);
            },

            previous: function () {
                if (this.page == 1) {
                    return;
                }
                var page = this.page;
                var rooms = this.rooms;
                this.page -= 1;
                getApi('/api/room', {
                    page: page - 1,
                    limit: 40
                }, function (err, r) {
                    if (err) {
                        alert("Error paging: " + err.message);
                    } else {
                        rooms.splice(0, rooms.length);
                        for(var i in r) {
                            rooms.push(r[i]);
                        }
                    }
                });
            },

            next: function () {
                var page = this.page;
                var rooms = this.rooms;
                getApi('/api/room', {
                    page: page + 1,
                    limit: 40
                }, function (err, r) {
                    if (err) {
                        alert('Error paging: ' + err.message)
                    } else {
                        if (r.length > 0) {
                            rooms.splice(0, rooms.length);
                            for(var i in r) {
                                rooms.push(r[i]);
                            }
                            page += 1;
                        }
                    }
                })
            }
        }
        
    });
}

$(document).ready(function() {
    getApi('/api/customer', {
        page: 1,
        limit: 15
    }, function (err, r) {
        if (err) {
            alert('获取客户信息出错: ' + err.message);
        } else {
            customerView(r);
        }
    })
});

function customerView(customers) {
    var vm = new Vue({
        el: '#customer',
        data: {
            customers: customers,
            keyword: '',
            page: 1
        },
        methods: {
            update: function () {
                var customers = this.customers;
                getApi('/api/customer', {
                    keyword: this.keyword,
                    page: this.page,
                    limit: 15
                }, function (err, r) {
                    if (err) {
                        alert('获取客户列表出错 - ' + err.message);
                    } else {
                        customers.splice(0, customers.length);
                        for (var i in r) {
                            customers.push(r[i]);
                        }
                    }
                });
            },

            previous: function() {
                if (this.page == 1) {
                    return;
                }
                this.page -= 1;
                this.update();
            },

            next: function() {
                this.page += 1;
                var _this = this;

                getApi('/api/customer', {
                    keyword: this.keyword,
                    page: page,
                    limit: 15
                }, function (err, r) {
                    if (err) {
                        alert('获取客户列表出错 - ' + err.message);
                    } else {
                        if (r.length == 0) {
                            _this.page = _this.page - 1;
                        } else {
                            customers.splice(0, customers.length);
                            for(var i in r) {
                                customers.push(r[i]);
                            }
                        }
                    }
                });
            },
            
            getDate: function(ts) {
                return toDateTime(ts);
            },

            getStatus: function(state) {
                switch (state) {
                    case 0:
                        return '已预订';
                        break;

                    case 1:
                        return '已入住';
                        break;

                    case 2:
                        return '已结账';
                        break;

                    default:
                        return 'N/A';
                        break;
                }
            }
        }
    })
}

$(document).ready(function () {
    getApi('/api/group', {
        page: 1,
        limit: 15
    }, function (err, r) {
        if (err) {
            alert('获取团体列表出错 - ' + err.message);
        } else {
            groupView(r);
        }
    })
});

function groupView(groups) {
    var vm = new Vue({
        el: '#group',
        data: {
            groups: groups,
            keyword: '',
            page: 1,
            members: []
        },
        methods: {
            update: function () {
                var groups = this.groups;
                getApi('/api/group', {
                    keyword: this.keyword,
                    page: this.page,
                    limit: 15
                }, function (err, r) {
                    if (err) {
                        alert('获取团体列表出错 - ' + err.message);
                    } else {
                        groups.splice(0, groups.length);
                        for(var i in r) {
                            groups.push(r[i]);
                        }
                    }
                });
            },

            previous: function () {
                if (this.page <= 1) {
                    return;
                }

                this.page -= 1;
                update();
            },

            next: function () {
                var _this = this;
                this.page += 1;
                getApi('/api/group', {
                    keyword: this.keyword,
                    page: this.page,
                    limit: 15
                }, function (err, r) {
                    if (err) {
                        alert('获取团体列表出错 - ' + err.message);
                    } else {
                        if (r.length > 0) {
                            _this.groups.splice(0, _this.groups.length);
                            for(var i in r) {
                                _this.groups.push(r[i]);
                            }
                        } else {
                            _this.page -= 1;
                        }
                    }
                });
            },

            getDate: function(ts) {
                return toDateTime(ts);
            },

            getStatus: function(state) {
                switch (state) {
                    case 0:
                        return '已预订';
                    break;

                    case 1:
                        return '已入住';
                    break;

                    case 2:
                        return '已结账';
                    break;

                    default:
                        return 'N/A';
                    break;
                }
            },
            
            groupBook: function() {
                location.assign('/groupbook.html');
            },
            
            checkout: function (group) {
                
            },
            
            checkin: function (group) {
                location.assign('/group_checkin.html?group=' + group.id);
            },
            
            groupDetail: function (group) {
                var member_ids = group.members.split(',');
                var members = this.members;
                members.splice(0, members.length);

                for(var i in member_ids) {
                    getApi('/api/customer/' + member_ids[i], {}, function (err, r) {
                        if (err) {

                        } else {
                            members.push(r);
                        }
                    });
                }

                $('#group-detail').modal('show');
            }
        }
    })
}

$(document).ready(function () {
    getApi('/api/item', {
        page: 1,
        limit: 15
    }, function (err, r) {
        if (err) {
            alert ('加载商品列表出错 - ' + err.message);
        } else {
            itemView(r);
        }
    })
});

function itemView (items) {
    var vm = new Vue({
        el: '#item',
        data: {
            items: items,
            keyword: '',
            page: 1
        },
        methods: {
            update: function () {
                var items = this.items;
                getApi('/api/item', {
                    keyword: this.keyword,
                    page: this.page,
                    limit: 15
                }, function (err, r) {
                    if (err) {
                        alert('加载商品列表出错: ' + err.message);
                    } else {
                        items.splice(0, items.length);
                        for(var i in r) {
                            items.push(r[i]);
                        }
                    }
                });
            },

            previous: function () {
                if (this.page <= 1) {
                    return;
                }
                this.page -= 1;
                this.update();
            },

            next: function () {
                var _this = this;
                this.page += 1;

                getApi('/api/item', {
                    keyword: this.keyword,
                    page: this.page,
                    limit: 15
                }, function (err, r) {
                    if (err) {
                        alert('Error loading next page: ' + err.message);
                    } else {
                        if (r.length > 0) {
                            _this.items.splice(0, _this.items.length);
                            for(var i in r) {
                                _this.items.push(r[i]);
                            }
                        }
                    }
                });
            }
        }
    })
}