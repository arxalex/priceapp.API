Vue.component('packageInsert', {
    template: `
        <div class="bg-white position-fixed p-4 window-insert shadow-lg rounded">
            <div class="d-flex mb-3"><h5 class="ms-1 fw-bold flex-fill">Package</h5>
                <button class="btn mt-0 pt-0 px-0" @click="close">
                    <i class="bi bi-x text-danger"></i>
                </button>
            </div>
            <div class="d-flex mb-3">
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Id</label>
                    <input class="form-control" v-model="package.id" placeholder="Id">
                </div>
                <div class="position-relative flex-fill me-2">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Label</label>
                    <input class="form-control" v-model="package.label">
                </div>
                <div class="position-relative flex-fill">
                    <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Short</label>
                    <input class="form-control" v-model="package.short">
                </div>
            </div>
            <div class="input-group">
                <button class="btn btn-primary form-control" v-on:click='insertOrUpdate'>Insert or Update</button>
            </div>
        </div>
    `,
    data() {
        return {
            package: this.sourcePackage
        }
    },
    props: {
        sourcePackage: {
            type: Object
        },
    },
    methods: {
        insertOrUpdate: async function () {
            const insertUrl = "../be/packages/insert_packages";
            const labelsUrl = "../be/items/get_labels";
            var data = {
                method: "InsertOrUpdatePackage",
                package: this.package
            }
            await this.getItemsFromDb(insertUrl, data);

            var labels = await this.getItemsFromDb(labelsUrl, {
                method: "GetAllLabels"
            });
            Vue.prototype.$labels = labels;
            this.$labels = labels;
            this.$emit("packageInserted");
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
        close: async function () {
            const labelsUrl = "../be/items/get_labels";
            var labels = await this.getItemsFromDb(labelsUrl, {
                method: "GetAllLabels"
            });
            Vue.prototype.$labels = labels;
            this.$labels = labels;
            this.$emit("packageInserted");
        }
    }
});

