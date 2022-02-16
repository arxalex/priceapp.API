Vue.component('Items', {
    template: `
        <div>
            <h1>Prepare items</h1>
                <p>List:
                    <div v-for="shop in shops">
                        <span>{{ shop.id }}</span> - <span>{{ shop.label }}</span>
                    </div>
                </p>
                <div class="input-group mb-3">
                    <input class="form-control" v-model="selectedShopId" type="number" placeholder="shop">
                </div>
                <div class="input-group mb-3">
                    <button @click="get_items()" class="btn btn-primary mt-2 w-100">Get items</button>
                </div>
                <div class="col-sm-12">
                    <item class="mb-3" v-for="item in items" :item="item"></item>
                <div>
                    <button class="btn btn-primary w-100" @click="save">Insert</button>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            shops: {
                silpo: {
                    label: 'silpo',
                    id: 1,
                },
                atb: {
                    label: 'atb',
                    id: 3,
                },
                auchan: {
                    label: 'auchan',
                    id: 2,
                }
            },
            selectedShopId: null,
            items: []
        }
    },
    /*props: {
        selectedShopId: {
            type: Number,
        },
        shops: {
            type: Array,
        },
        items: {
            type: Array,
        },
    },*/
    /*mounted() {
        this.selectedShopId = null;
        this.shops = {
            silpo: {
                label: 'silpo',
                id: 1,
            },
            atb: {
                label: 'atb',
                id: 3,
            },
            auchan: {
                label: 'auchan',
                id: 2,
            },
        };
        this.items = [];
    },*/
    computed: {

    },
    methods: {
        get_items: function () {
            var shop = this.selectedShopId;
            var url = "";
            var data = {};
            switch (shop) {
                case 1:
                    url = "be/items/get_item";
                    data = {
                        source: 1,
                        category: 425
                    };
                    break;
                case 2:
                    url = "../get_items.php";
                    data = {
                        shop: 2,
                    };
                    break;
                case 3:
                    url = "../get_items.php";
                    data = {
                        shop: 3,
                    };
                    break;
            }
            var items = this.getItemsFromDb(url, data);
            items.array.forEach(item => {
                this.item.push(item);
            });
            /*this.items.push({
                label: "item",
                image: "https://content.silpo.ua/sku/ecommerce/default/480x480/3_480x480.png",
                category: 0,
                categoryLabel: "undefined",
                brand: 0,
                brandLabel: "undefined",
                package: 1,
                packageLabel: "кг",
                units: 1,
                term: 1,
                barcodes: [],
                consist: [],
                consistLabels: [],
                calorie: null,
                carbohydrates: null,
                fat: null,
                proteins: null,
                additional: {
                    country: 1,
                },
                additionalLabels: {
                    country: "Україна",
                },
                itemSimId: 0,
                itemSim: {
                    label: "item",
                    image: "https://content.silpo.ua/sku/ecommerce/default/480x480/3_480x480.png",
                    category: 0,
                    categoryLabel: "undefined",
                    brand: 0,
                    brandLabel: "undefined",
                    package: 1,
                    packageLabel: "кг",
                    units: 1,
                    term: 1,
                    barcodes: [],
                    consist: [],
                    consistLabels: [],
                    calorie: null,
                    carbohydrates: null,
                    fat: null,
                    proteins: null,
                    additional: {
                        country: 1,
                    },
                    additionalLabels: {
                        country: "Україна",
                    },
                }
            })*/
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    console.log(response.data);
                    return response.data;
                }
            });
        },
        save: function () {

        }
    },
});

