/**
 * Created by Excelle on 7/7/16.
 */


$(document).ready(function () {
    var args = getArgs();
    if (!args.room) {
        location.assign('/');
    } else {
        getApi('/api/room/' + args.room, {}, function (err, r) {
            if (err) {
                alert('加载房间信息错误: ' + err.message);
            } else {
                getApi('/api/roomtype/' + r.type, {}, function (err2, rt) {
                    if (err2) {
                        alert('Error loading room type - ' + err2.message);
                    } else {
                        reservationView({
                            id: r.id,
                            name: r.name,
                            typename: rt.name,
                            price: (r.custom_Price - 1e-4 < 0) ? rt.typical_Price : r.custom_Price
                        });
                    }
                });
            }
        });
    }
});


function reservationView(room) {
    var vm = new Vue({
        el: '#reserve-form',
        data: {
            room: room,
            customers: [],
            name: '',
            identity: '',
            gender: 1
        },
        methods: {
            appendCustomer: function () {
                $('#add-customer').modal('show');
            },
            
            append: function () {
                this.customers.push({
                    name: this.name,
                    identity: this.identity,
                    gender: (this.gender == 1),
                    room_id: this.room.id
                });
                $('#add-customer').modal('hide');
                this.name = this.identity = '';
            },

            getGender: function (val) {
                return val ? '男' : '女';
            },

            submit: function () {
                if (this.customers.length == 0) {
                    return;
                }
                postApi('/api/room/' + this.room.id + '/customers',
                    this.customers, function (err, r) {
                        if (err) {
                            alert('提交预订发生错误: ' + err.message);
                        } else {
                            alert('预订成功!');
                        }
                        history.back();
                    });
            }
        }
    })
}