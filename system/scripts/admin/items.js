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
                <item class="mb-3" 
                    v-for="itemModel in itemModels" 
                    :item="itemModel.item" 
                    :similarItems="itemModel.similarItems"
                    :labels="itemModel.labels"
                    :similarLabels="itemModel.similarLabels">
                </item>
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
            itemModels: [],
        }
    },
    methods: {
        get_items: async function () {
            const similarUrl = "../be/items/get_similar_items";
            const labelsUrl = "../be/items/get_labels";
            var shop = this.selectedShopId;
            var url = "";
            var data = {};
            switch (shop) {
                case "1":
                    url = "../be/items/get_items";
                    data = {
                        source: 1,
                        category: 425
                    };
                    break;
                case "2":
                    url = "../get_items.php";
                    data = {
                        shop: 2,
                    };
                    break;
                case "3":
                    url = "../get_items.php";
                    data = {
                        shop: 3,
                    };
                    break;
            }
            var items = await this.getItemsFromDb(url, data);

            var itemLabels = [];
            items.forEach(element => {
                itemLabels.push(element.label);
            });
            var similarItems = await this.getItemsFromDb(similarUrl, {
                itemLabels: itemLabels
            });

            var labelsRequest = [];
            items.forEach(element => {
                labelsRequest.push({
                    categoryId: element.category,
                    brandId: element.brand,
                    packageId: element.package,
                    consistIds: element.consist,
                    countryId: element.additional.country
                });
            });
            var labels = await this.getItemsFromDb(labelsUrl, {
                labelIds: labelsRequest
            });

            var similarLabels = [];
            
            for(var i = 0; i < similarItems.length; i++){
                var similarLabelRequests = [];
                similarItems[i].forEach(element => {
                    similarLabelRequests.push({
                        categoryId: element.category,
                        brandId: element.brand,
                        packageId: element.package,
                        consistIds: element.consist,
                        countryId: element.additional.country
                    });
                });
                var similarLabel = await this.getItemsFromDb(labelsUrl, {
                    labelIds: similarLabelRequests
                });
                similarLabels.push(similarLabel);
            }

            for(var i = 0; i < items.length; i++){
                this.itemModels.push({
                    item: items[i],
                    similarItems: similarItems[i],
                    labels: labels[i],
                    similarLabels: similarLabels[i]
                });
            }
            console.log(this.itemModels);
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
        save: function () {

        }
    },
});

