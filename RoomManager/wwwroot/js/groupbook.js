/**
 * Created by Excelle on 7/7/16.
 */

$(document).ready(groupbookView);


function groupbookView() {
    var vm = new Vue({
        el: '#groupbook',
        data: {
            members: [],
            available_rooms: [],
            member_ids: '',
            leader_id: '',
            selected_room_index: 0,
            currentMem: [],
            name: '',
            identity: '',
            gender: 1
        },
        methods: {
            showAdd: function () {
                var rooms = this.available_rooms;
                $('#add-member').modal({
                    backdrop: 'static'
                }).modal('show');

                getApi('/api/room', {
                    specific: 'vacant'
                }, function (err, r) {
                    if (err) {
                        alert('Error retrieving available rooms. ' + err.message);
                    } else {
                        rooms.splice(0, rooms.length);
                        for(var i in r) {
                            rooms.push(r[i]);
                        }
                    }
                });
            },

            getGender: function(val) {
                return val == 1 ? '男' : '女';
            },

            pushmember: function () {
                if (this.currentMem.length >= this.available_rooms[this.selected_room_index-1].capacity) {
                    return alert('This room is empty.');
                }

                var data = {
                    name: this.name,
                    identity: this.identity,
                    gender: parseInt(this.gender),
                    room_Id: parseInt(this.available_rooms[this.selected_room_index-1].id)
                };
                this.currentMem.push(data);
            },

            getRmNum: function (id) {
                return this.available_rooms[id].number;
            },

            participate: function () {
                if (this.currentMem.length == 0) {
                    return;
                }
                var room_id = this.available_rooms[this.selected_room_index-1].id;
                var currentMems = this.currentMem;
                var mems = this.members;

                postApi('/api/room/' + room_id + '/customers',
                    this.currentMem, function (err, r) {
                        if (err) {
                            alert('Error participating: ' + err.message);
                        } else {
                            currentMems.splice(0, currentMems.length);
                            for(var i in r) {
                                mems.push(r[i]);
                            }
                            $('#add-member').modal('hide');
                        }
                    });
            },

            submit: function () {
                var members = this.members;
                var member_ids = [];
                if (members.length == 0) {
                    return;
                }

                for(var i in members) {
                    member_ids.push(members[i].id.toString());
                }
                var data = {
                    leader_Id: parseInt(members[0].id),
                    members: member_ids.join(',ls' +
                        ''),
                    status: 0
                };

                putApi('/api/group', data, function (err, r) {
                    if (err) {
                        alert('Error booking for a group = ' + err.message);
                    } else {
                        alert('团体预订成功!');
                        location.assign('/');
                    }
                });
            }
        }
    });
}